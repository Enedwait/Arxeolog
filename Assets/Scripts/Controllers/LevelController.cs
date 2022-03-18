// =============================================================================== \\
//                      © Oleg Tolmachev [OKRT] 2022                               \\
// =============================================================================== \\
using System;
using System.ComponentModel;
using System.IO;
using Arχæolog.Classes.Data;
using Arχæolog.Scripts.Data;
using Arχæolog.Scripts.Objects;
using UnityEngine;
using UnityEngine.Tilemaps;
using Random = UnityEngine.Random;

namespace Arχæolog.Scripts.Controllers
{
    /// <summary>
    /// The <see cref="LevelController"/> class.
    /// </summary>
    internal sealed class LevelController : PersistableObject
    {
        #region Serialized Fields

        [SerializeField] private Camera mainCamera;
        [SerializeField] private PlayerController player;
        [SerializeField] private int fieldSize = 10;
        [SerializeField] private int cellDepth = 3;
        [SerializeField] private int amountOfShowels = 10;
        [SerializeField] private int amountOfGoldBarsToWin = 3;
        [SerializeField] private Tilemap tilemap;
        [SerializeField] private Tile tileGround;
        [SerializeField] private Tile tileBedRock;
        [SerializeField] private Item goldBar;
        [SerializeField, Range(0f, 1f)] private float findGoldBarProbability = 0.5f;
        [SerializeField] private Sack sack;
        [SerializeField] private Transform itemPool;
        [SerializeField] private PersistentStorage storage;

        #endregion

        #region Fields

        [EditorBrowsable(EditorBrowsableState.Never)]
        private Random.State _mainRandomState;

        private int _maxFieldSize = 10;
        private int _maxCellDepth = 10;

        #endregion

        #region Properties

        /// <summary> Gets the <see cref="LevelController"/> instance. </summary>
        public static LevelController Instance { get; private set; }

        /// <summary> Gets the victory flag. </summary>
        public bool IsWon { get; private set; }

        #endregion

        #region Lifecycle Methods

        void Awake()
        {
            Instance = this;

            player.Dug += Player_Dug;
            sack.Changed += Sack_Changed;
        }

        void Start()
        {
            Random.InitState((int)DateTime.UtcNow.Ticks);
            _mainRandomState = Random.state;

            if (File.Exists(storage.SavePath))
                Load();
            else
                Restart();
        }

        void OnApplicationQuit()
        {
            Save();
        }

        #endregion

        #region GameState

        /// <summary>
        /// Restarts the level.
        /// </summary>
        public void Restart()
        {
            IsWon = false;

            SetRandomState();

            Generate();

            sack.Clear();
            player.Showel.count = amountOfShowels;

            UIController.Instance.Restart();

            CheckWin();
        }

        /// <summary>
        /// Checks the possibility of victory.
        /// </summary>
        private void CheckWin()
        {
            IsWon = false;

            if (sack.GoldBar.count >= amountOfGoldBarsToWin)
                Win();
        }

        /// <summary>
        /// Win the level.
        /// </summary>
        private void Win()
        {
            IsWon = true;
            UIController.Instance.Win();
        }

        /// <summary>
        /// Checks the possibility of defeat.
        /// </summary>
        private void CheckLose()
        {
            if (player.Showel.count <= 0 && itemPool.GetComponentsInChildren<Item>().Length == 0)
                Lose();
        }

        /// <summary>
        /// Lose the level.
        /// </summary>
        private void Lose()
        {
            UIController.Instance.Lose();
        }

        #endregion

        #region Methods

        /// <summary>
        /// Sets the random state.
        /// </summary>
        private void SetRandomState()
        {
            Random.state = _mainRandomState;
            int seed = Random.Range(0, int.MaxValue) ^ (int)Time.unscaledTime;
            _mainRandomState = Random.state;
            Random.InitState(seed);
        }

        /// <summary>
        /// Clears the item pool.
        /// </summary>
        private void ClearItemPool()
        {
            Item[] items = itemPool.GetComponentsInChildren<Item>();
            foreach (var item in items)
                Destroy(item.gameObject);
        }

        /// <summary>
        /// Validates start data.
        /// </summary>
        void ValidateData()
        {
            if (fieldSize <= 0 || fieldSize > _maxFieldSize)
                fieldSize = _maxFieldSize;

            if (cellDepth <= 0 || cellDepth > _maxCellDepth)
                cellDepth = _maxCellDepth;

            if (amountOfShowels <= 0)
                amountOfShowels = 20;

            if (amountOfGoldBarsToWin <= 0)
                amountOfGoldBarsToWin = 3;
        }

        /// <summary>
        /// Gets the world position through the given screen position.
        /// </summary>
        /// <param name="onScreenPosition">on screen position.</param>
        /// <returns>world position.</returns>
        public Vector3 GetWorldPosition(Vector2 onScreenPosition) => mainCamera.ScreenToWorldPoint(onScreenPosition);

        /// <summary>
        /// Checks if it's a chance.
        /// </summary>
        /// <param name="chance">chance probability.</param>
        /// <returns><value>True</value> if it's a chance; otherwise <value>False</value>.</returns>
        public bool IsChance(float chance)
        {
            int random = Random.Range(0, 10001);
            return random >= 0 && random < (int)(chance * 10000);
        }

        #endregion

        #region Storage

        /// <summary>
        /// Saves the data.
        /// </summary>
        /// <param name="writer">writer.</param>
        public override void Save(GameDataWriter writer)
        {
            // write save file version
            writer.Write((int)1);

            // write the field size
            writer.Write((int)fieldSize);

            // write the cell depth
            writer.Write((int)cellDepth);

            // write the count of showels
            writer.Write((int)amountOfShowels);

            // write the amount of gold bars to win
            writer.Write((int)amountOfGoldBarsToWin);

            // write the probability
            writer.Write((float)findGoldBarProbability);

            // write the random state
            writer.Write(Random.state);

            // write the tilemap data
            for (int i = 0; i < fieldSize; i++)
            for (int j = 0; j < fieldSize; j++)
            for (int k = 0; k < cellDepth; k++)
            {
                var tile = tilemap.GetTile(new Vector3Int(i - fieldSize / 2, j - fieldSize / 2, k)) as Tile;
                if (tile != null)
                {
                    if (tile.Equals(tileGround)) // write the tile type
                        writer.Write((byte)1);
                }
                else writer.Write((byte)0); // write the empty tile
            }

            Item[] items = itemPool.GetComponentsInChildren<Item>();
            // write count of gold bars
            writer.Write((int)items.Length);
            if (items.Length > 0)
                foreach (var item in items)
                    item.Save(writer);

            // write the sack data
            sack.Save(writer);

            // write the player data
            player.Save(writer);
        }

        /// <summary>
        /// Saves the data.
        /// </summary>
        public void Save() => storage.Save(this);

        /// <summary>
        /// Loads the data.
        /// </summary>
        /// <param name="reader">reader.</param>
        public override void Load(GameDataReader reader)
        {
            ClearItemPool();
            ClearTilemap();

            // read save file version
            int version = reader.ReadInt();
            if (version == 1)
            {
                // read the field size
                fieldSize = reader.ReadInt();

                // read the cell depth
                cellDepth = reader.ReadInt();

                // read the amount of showels
                amountOfShowels = reader.ReadInt();

                // read the amount of gold bars to win
                amountOfGoldBarsToWin = reader.ReadInt();

                // read the gold bar probability
                findGoldBarProbability = reader.ReadFloat();

                // read the randomizer state
                Random.state = reader.ReadRandomState();

                // read the tilemap data
                for (int i = 0; i < fieldSize; i++)
                for (int j = 0; j < fieldSize; j++)
                {
                    for (int k = 0; k < cellDepth; k++)
                    {
                        Tile tile = null;
                        switch (reader.ReadByte())
                        {
                            case 1:
                                tile = tileGround;
                                break;
                        }

                        tilemap.SetTile(new Vector3Int(i - fieldSize / 2, j - fieldSize / 2, k), tile);
                    }

                    tilemap.SetTile(new Vector3Int(i - fieldSize / 2, j - fieldSize / 2, -1), tileBedRock);
                }

                int countOfGoldBarsInPool = reader.ReadInt();
                for (int i = 0; i < countOfGoldBarsInPool; i++)
                {
                    Item newGoldBar = Instantiate(goldBar, Vector3.zero, Quaternion.identity, itemPool);
                    newGoldBar.Load(reader);
                }

                // read the sack data
                sack.Load(reader);

                // read the player data
                player.Load(reader);
            }

            UIController.Instance.Restart();
            CheckWin();
        }

        /// <summary>
        /// Loads the data.
        /// </summary>
        public void Load() => storage.Load(this);

        #endregion

        #region Tilemap

        /// <summary>
        /// Generates the level.
        /// </summary>
        void Generate()
        {
            ClearItemPool();
            ClearTilemap();

            ValidateData();

            for (int i = 0; i < fieldSize; i++)
            for (int j = 0; j < fieldSize; j++)
                FillCell(i - fieldSize / 2, j - fieldSize / 2, cellDepth);
        }

        /// <summary>
        /// Clears the tilemap entirely.
        /// </summary>
        void ClearTilemap()
        {
            for (int i = 0; i < _maxFieldSize; i++)
            for (int j = 0; j < _maxFieldSize; j++)
                ClearCell(i - _maxFieldSize / 2, j - _maxFieldSize / 2, _maxCellDepth);
        }

        /// <summary>
        /// Fills the specified cell.
        /// </summary>
        /// <param name="i">row.</param>
        /// <param name="j">column.</param>
        /// <param name="depth">depth.</param>
        void FillCell(int i, int j, int depth)
        {
            tilemap.SetTile(new Vector3Int(i, j, -1), tileBedRock);
            for (int k = 0; k < depth; k++)
                tilemap.SetTile(new Vector3Int(i, j, k), tileGround);
        }

        /// <summary>
        /// Clears the specified cell entirely.
        /// </summary>
        /// <param name="i">row.</param>
        /// <param name="j">column.</param>
        /// <param name="depth">depth.</param>
        void ClearCell(int i, int j, int depth)
        {
            tilemap.SetTile(new Vector3Int(i, j, -1), null);
            for (int k = 0; k < depth; k++)
                tilemap.SetTile(new Vector3Int(i, j, k), null);
        }

        /// <summary>
        /// Checks if the world position is occupied by base tile (z = -1).
        /// </summary>
        /// <param name="world">world position</param>
        /// <param name="center">center of the tile.</param>
        /// <returns><value>True</value> if occupied by tile; otherwise <value>False</value>.</returns>
        public bool HasTileBelow(Vector3 world, out Vector3 center)
        {
            center = Vector3.zero;

            Vector3Int cell = tilemap.WorldToCell(world);
            Vector3Int baseCell = new Vector3Int(cell.x, cell.y, -1);
            TileBase tile = tilemap.GetTile(baseCell);
            if (tile != null)
            {
                center = tilemap.GetCellCenterWorld(cell);
                return true;
            }

            return false;
        }

        /// <summary>
        /// Digs at the specified world position.
        /// </summary>
        /// <param name="world">world position.</param>
        /// <returns><value>True</value> if dug successfully; otherwise <value>False</value>.</returns>
        public bool DigAt(Vector3 world)
        {
            if (IsWon)
                return false;

            Vector3Int cell = tilemap.WorldToCell(world);

            // iterate through all the depth and determine if it's possible to dig
            for (int i = cellDepth - 1; i >= 0; i--)
            {
                Vector3Int currentCell = new Vector3Int(cell.x, cell.y, i);
                TileBase tile = tilemap.GetTile(currentCell);
                // check if tile found
                if (tile != null) 
                {
                    // check if nothing prevents us from digging the current tile
                    var hit = Physics2D.Raycast(tilemap.GetCellCenterWorld(cell), Vector2.zero);
                    if (hit.transform != null)
                    {
                        string layer = LayerMask.LayerToName(hit.transform.gameObject.layer);
                        // check if we have an item on this tile
                        if (layer.Equals("Item")) // then we can't dig here
                            return false;
                    }

                    // dig
                    tilemap.SetTile(currentCell, null);

                    // check if it's our chance to find a gold bar
                    if (IsChance(findGoldBarProbability))
                        Instantiate(goldBar, tilemap.GetCellCenterWorld(cell), Quaternion.identity, itemPool);

                    return true;
                }
            }

            return false;
        }

        #endregion

        #region Events Handling

        /// <summary>
        /// Handles the 'Change' event of the sack.
        /// </summary>
        /// <param name="sender">sender.</param>
        /// <param name="e">event args.</param>
        private void Sack_Changed(object sender, EventArgs e) => CheckWin();

        /// <summary>
        /// Handles the 'Dug' event of the player.
        /// </summary>
        /// <param name="sender">sender.</param>
        /// <param name="e">event args.</param>
        private void Player_Dug(object sender, EventArgs e) => CheckLose();

        #endregion
    }
}
