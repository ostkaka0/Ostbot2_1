using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using PlayerIOClient;
using Skylight;

namespace OstBot_2_1
{
    public class Room : SubBot
    {
        public List<Block>[][,] blockMap = new List<Block>[2][,];
        public  Queue<Block> blockQueue = new Queue<Block>();
        public  Queue<Block> blockRepairQueue = new Queue<Block>();
        public  HashSet<Block> blockSet = new HashSet<Block>();

        public int width;
        public int height;
        int drawSleep = 8;

        bool blockDrawerEnabled = false;

        public Room(OstBot ostBot)
            : base()
        {
            enabled = true;
        }

        public void setDrawSleep(int drawSleep)
        {
            this.drawSleep = drawSleep;
        }

        public void DrawBlock(Block block)
        {
            if (block == null)
                return;
            if (Block.Compare(getBotMapBlock(block.layer, block.x, block.y), block))
                return;

            lock (blockSet)
            {
                //if (blockSet.Contains(block))
                //    return;
                foreach (Block b in blockSet)
                {
                    if (block == b)//(false && block.Equals(b))
                    {
                        //Console.
                        //("== failar inte>.<");
                        return;
                    }
                    else if (b.layer == block.layer && b.x == block.x && b.y == block.y)
                    {
                        blockSet.Remove(b);
                        break;
                    }
                }


                blockSet.Add(block);
            }

            //Console.WriteLine("boo");

            lock (blockQueue)
                blockQueue.Enqueue(block);
        }

        public Block getMapBlock(int layer, int x, int y, int rollbacks)
        {
            while (blockMap == null)
                Thread.Sleep(100);

            while (blockMap[layer] == null)
                Thread.Sleep(100);


            if (x >= 0 && y >= 0 && x < width && y < height)
            {
                lock (blockMap)
                {
                    if (blockMap[layer][x, y].Count > 0)
                    {
                        if (blockMap[layer][x, y].Count <= rollbacks)
                            return Block.CreateBlock(layer, x, y, 0, -1);
                        else
                            return blockMap[layer][x, y][blockMap[layer][x, y].Count - 1 - rollbacks];
                    }
                }
            }
            return Block.CreateBlock(layer, x, y, 0, -1);
        }

        public Block getBotMapBlock(int layer, int x, int y)
        {
            if (x >= 0 && y >= 0 && x < width && y < height)
            {
                lock (blockSet)
                {
                    foreach (Block b in blockSet)
                    {
                        if (b.x == x && b.y == y && b.layer == layer)
                            return b;
                    }
                }

                while (blockMap == null)
                    Thread.Sleep(100);

                while (blockMap[layer] == null)
                    Thread.Sleep(100);

                lock (blockMap)
                {
                    if (blockMap[layer][x, y].Count > 0)
                    {
                        return blockMap[layer][x, y][blockMap[layer][x, y].Count - 1];
                    }
                }
            }
            return Block.CreateBlock(layer, x, y, 0, -1);
        }


        public void DrawBorder()
        {
            for (int x = 0; x < width; x++)
            {
                for (int y = 0; y < height; y++)
                {
                    if (x == 0 || y == 0 || x == width - 1 || y == width - 1)
                    {
                        blockMap[0][x, y].Clear();
                        blockMap[0][x, y].Add(Block.CreateBlock(0, x, y, 9, -1));
                        //Console.WriteLine("Border at " + x + " " + y);
                    }
                }
            }
        }

        public override void onMessage(object sender, OstBot ostBot, PlayerIOClient.Message m)
        {
            //try
            //{
            switch (m.Type)
            {
                case "init":
                    bool isOwner;
                    if (OstBot.isBB)
                    {
                        //worldKey = rot13(m[3].ToString());
                        //botPlayerID = m.GetInt(6);
                        width = m.GetInt(10);
                        height = m.GetInt(11);
                        //hasCode = m.GetBoolean(8);
                        isOwner = m.GetBoolean(9);
                    }
                    else
                    {
                        //worldKey = rot13(m[5].ToString());
                        //botPlayerID = m.GetInt(6);
                        width = m.GetInt(12);
                        height = m.GetInt(13);
                        //hasCode = m.GetBoolean(10);
                        isOwner = m.GetBoolean(11);
                    }


                    if (isOwner)
                        BlockDrawer();

                    lock (blockMap)
                    {
                        for (int l = 0; l < 2; l++)
                        {
                            blockMap[l] = new List<Block>[width, height];

                            for (int x = 0; x < width; x++)
                            {
                                for (int y = 0; y < height; y++)
                                {
                                    blockMap[l][x, y] = new List<Block>();
                                }
                            }
                        }
                    }

                    LoadMap(m, 18);
                    break;

                case "reset":

                    lock (blockMap)
                    {
                        for (int l = 0; l < 2; l++)
                        {
                            blockMap[l] = new List<Block>[width, height];

                            for (int x = 0; x < width; x++)
                            {
                                for (int y = 0; y < height; y++)
                                {
                                    blockMap[l][x, y] = new List<Block>();
                                }
                            }
                        }
                    }
                    LoadMap(m, 0);
                    break;

                case "access":
                    Thread.Sleep(5);
                    BlockDrawer();
                    break;

                case "b":
                    {
                        Block block;

                        while (blockMap == null)
                            Thread.Sleep(5);

                        while (blockMap[m.GetInt(0)] == null)
                            Thread.Sleep(5);

                        Block newBlock = new Block(m);

                        lock (blockMap)
                            blockMap[m.GetInt(0)][m.GetInt(1), m.GetInt(2)].Add(newBlock);

                        if (m.Count >= 5)
                            block = Block.CreateBlock(m.GetInt(0), m.GetInt(1), m.GetInt(2), m.GetInt(3), m.GetInt(4));
                        else
                            block = Block.CreateBlock(m.GetInt(0), m.GetInt(1), m.GetInt(2), m.GetInt(3), -1);

                        OnBlockDraw(block);

                    }
                    break;

                case "bc": // bc, bs, pt, lb, br
                    {
                        Block block;

                        while (blockMap == null)
                            Thread.Sleep(5);

                        while (blockMap[0] == null)
                            Thread.Sleep(5);

                        block = new Block(m);

                        lock (blockMap)
                            blockMap[0][m.GetInt(0), m.GetInt(1)].Add(block);

                        OnBlockDraw(block);
                    }
                    break;

                case "bs":
                    goto case "bc";

                case "pt":
                    goto case "bc";

                case "lb":
                    goto case "bc";

                case "br":
                    goto case "bc";

                case "clear":
                    {
                        //Redstone.ClearLists();
                        for (int x = 0; x < width; x++)
                        {
                            for (int y = 0; y < height; y++)
                            {
                                for (int i = 0; i < blockMap.Length; i++)
                                {
                                    blockMap[i][x, y].Add(Block.CreateBlock(0, x, y, 0, 0));
                                }
                            }
                        }
                        DrawBorder();

                    }
                    break;


            }
            //}
            //catch (Exception e)
            //{
            // OstBot.shutdown();
            //throw e;
            //}
        }

        public override void onDisconnect(object sender, string reason)
        {
            blockMap = new List<Block>[2][,];
            blockQueue = new Queue<Block>();
            blockRepairQueue = new Queue<Block>();
            blockSet = new HashSet<Block>();
        }

        public override void onCommand(object sender, OstBot ostBot, string text, string[] args, int userId, Player player, string name, bool isBotMod)
        {

        }

        public override void Update()
        {

        }

        private void OnBlockDraw(Block block)
        {
            lock (blockSet)
            {
                if (blockSet.Contains(block))
                    blockSet.Remove(block);
            }

            lock (blockSet)
            {
                foreach (Block b in blockSet)
                {
                    if (block.Equals(b))
                    {
                        lock (blockMap)
                        {
                            blockMap[block.layer][block.x, block.y].Add(b);
                        }

                        blockSet.Remove(b);
                        break;
                    }
                    /*if (Block.Compare(block, b))
                    {
                        if (b.x == block.x && b.y == block.y)
                        {
                            blockSet.Remove(b);
                        }
                    }*/
                }
            }
        }

        private void LoadMap(Message m, uint position)
        {
            lock (blockMap)
            {
                byte[] xByteArray;
                byte[] yByteArray;
                for (uint i = position; i < m.Count; i++)
                {
                    if (m[i] is byte[])
                    {
                        int blockID = m.GetInt(i - 2);
                        int layer = m.GetInt(i - 1);
                        xByteArray = m.GetByteArray(i);
                        yByteArray = m.GetByteArray(i + 1);
                        int xIndex = 0;
                        int yIndex = 0;
                        i += 2;
                        for (int x = 0; x < xByteArray.Length; x += 2)
                        {
                            xIndex = (xByteArray[x] * 256) + xByteArray[x + 1];
                            yIndex = (yByteArray[x] * 256) + yByteArray[x + 1];

                            Block block;

                            switch (blockID)
                            {
                                case BlockIds.Action.Doors.COIN:
                                    {
                                        int coinsToOpen = m.GetInt(i);
                                        block = Block.CreateBlockCoin(xIndex, yIndex, blockID, coinsToOpen);
                                        break;
                                    }
                                case BlockIds.Action.Gates.COIN:
                                    goto case BlockIds.Action.Doors.COIN;

                                case BlockIds.Action.Music.PIANO:
                                    {
                                        int soundId = m.GetInt(i);
                                        block = Block.CreateNoteBlock(xIndex, yIndex, blockID, soundId);
                                        break;
                                    }
                                case BlockIds.Action.Music.PERCUSSION:
                                    goto case BlockIds.Action.Music.PIANO;

                                case BlockIds.Action.Portals.NORMAL:
                                    {
                                        int rotation = m.GetInt(i);
                                        int id = m.GetInt(i + 1);
                                        int target = m.GetInt(i + 1);
                                        block = Block.CreatePortal(xIndex, yIndex, rotation, id, target);
                                        break;
                                    }
                                case BlockIds.Action.Portals.INVISIBLE:
                                    goto case BlockIds.Action.Portals.NORMAL;

                                case 1000:
                                    {
                                        string text = m.GetString(i);
                                        block = Block.CreateText(xIndex, yIndex, text);
                                    }
                                    break;

                                case BlockIds.Action.Hazards.SPIKE:
                                    {
                                        int rotation = m.GetInt(i);
                                        block = Block.CreateSpike(xIndex, yIndex, rotation);
                                        break;
                                    }

                                default:
                                    block = Block.CreateBlock(layer, xIndex, yIndex, blockID, -1);
                                    break;

                            }
                            blockMap[layer][xIndex, yIndex].Add(block);//blockID;
                        }
                    }
                }
                DrawBorder();
            }
        }


        private void BlockDrawer()
        {
            OstBot.connection.Send(OstBot.worldKey + "k", true);
            if (!blockDrawerEnabled)
            {
                blockDrawerEnabled = true;
                new Thread(() =>
                {
                    try
                    {

                        Stopwatch stopwatch = new Stopwatch();
                        stopwatch.Start();

                        while (OstBot.connected)
                        {
                            while (OstBot.hasCode)
                            {

                                lock (blockQueue)
                                {
                                    if (blockQueue.Count != 0)
                                    {

                                        if (blockSet.Contains(blockQueue.Peek()))
                                        {
                                            //Console.WriteLine("jag är en sjuk sak");
                                            blockQueue.Peek().Send(OstBot.connection);
                                            lock (blockRepairQueue)
                                                blockRepairQueue.Enqueue(blockQueue.Dequeue());
                                            //Console.WriteLine("!!");
                                        }
                                        else
                                        {
                                            blockQueue.Dequeue();
                                            continue;
                                        }
                                    }
                                    else if (blockRepairQueue.Count != 0)
                                    {
                                        while (!blockSet.Contains(blockRepairQueue.Peek()))
                                        {
                                            blockRepairQueue.Dequeue();
                                            if (blockRepairQueue.Count == 0)
                                                break;
                                        }

                                        if (blockRepairQueue.Count == 0)
                                            continue;

                                        blockRepairQueue.Peek().Send(OstBot.connection);
                                        blockRepairQueue.Enqueue(blockRepairQueue.Dequeue());
                                    }
                                    else
                                    {
                                        Thread.Sleep(5);
                                        continue;
                                    }
                                    double sleepTime = drawSleep - stopwatch.Elapsed.TotalMilliseconds;
                                    if (sleepTime >= 0.5)
                                    {
                                        Thread.Sleep((int)sleepTime);
                                    }
                                    stopwatch.Reset();
                                }
                            }
                            Thread.Sleep(100);
                        }
                    }
                    catch (Exception e)
                    {
                        OstBot.shutdown();
                        throw e;
                    }

                }).Start();
            }
        }


    }
}
