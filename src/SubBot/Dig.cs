using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using Graphics.Tools.Noise;

namespace OstBot_2_
{
    public class Dig : SubBot
    {
        private void Generate(int width, int height)
        {
            Random random = new Random();
            Graphics.Tools.Noise.Primitive.SimplexPerlin noise = new Graphics.Tools.Noise.Primitive.SimplexPerlin(random.Next(), NoiseQuality.Best);
            //f.Heightmap.
            Console.WriteLine("sdfsdfsfdrgsadrgdsgsdfsdf");
            Block[,] blockMap = new Block[width, height];
            

            for (int x = 1; x < width - 1; x++)
            {
                for (int y = 30; y < height - 1; y++)
                {
                    if (noise.GetValue(x * 0.0625F, y * 0.0625F) < 0.5 - y/height*64)
                        blockMap[x, y] = Block.CreateBlock(0, x, y, Skylight.BlockIds.Blocks.Sand.BROWN, -1);
                    else
                        blockMap[x, y] = Block.CreateBlock(0, x, y, Skylight.BlockIds.Blocks.Sand.GRAY, -1);
                }
            }

            Queue<Block> blockQueue = new Queue<Block>();

            for (int j = 0; j < 7; j++ )
                for (int i = 0; i < 4; i++)
                    blockQueue.Enqueue(Block.CreateBlock(0, random.Next(1, width - 1), random.Next(1, height - 1), Skylight.BlockIds.Blocks.Minerals.RED+j, -1));

            int amount = 512;

            while (blockQueue.Count > 0 && amount > 0)
            {
                Block block = blockQueue.Dequeue();

                blockMap[block.x, block.y] = block;

                if (random.Next(8) == 0)
                {
                    Block block2 = Block.CreateBlock(block.layer, block.x, block.y, block.blockId, -1);

                    switch (random.Next(4))
                    {
                        case 0: block2.x = block2.x + 1; break;
                        case 1: block2.y = block2.y + 1; break;
                        case 2: block2.x = block2.x - 1; break;
                        case 3: block2.y = block2.y - 1; break;
                    }

                    Console.WriteLine("s");

                    if (!Block.Compare(blockMap[block2.x, block2.y], block2) && block2.x > 1 && block2.y > 1 && block2.x < width-1 && block2.y < height-1)
                    {
                        blockQueue.Enqueue(block2);
                        blockMap[block2.x, block2.y] = block2;
                        amount--;
                        Console.WriteLine(amount);
                    }
                }

                blockQueue.Enqueue(block);
            }

            for (int x = 1; x < width - 1; x++)
            {
                for (int y = 30; y < height - 1; y++)
                {
                    if (blockMap[x, y] != null)
                        OstBot.room.DrawBlock(blockMap[x, y]);
                }
            }
        }

        public void onMessage(object sender, PlayerIOClient.Message m)
        {
            Console.WriteLine("sfsddf");
            switch (m.Type)
            {
                case "init":
                    Generate(m.GetInt(10), m.GetInt(11));
                    break;

                case "say":
                    {
                        int userId = m.GetInt(0);
                        string text = m.GetString(1);
                        if (text.StartsWith("!"))
                        {
                            string[] arg = text.ToLower().Split(' ');
                            string name = "";
                            lock (OstBot.playerListLock)
                            {
                                if (OstBot.playerList.ContainsKey(userId))
                                    name = OstBot.playerList[userId].name;
                            }

                            switch (arg[0])
                            {
                                case "!generate":
                                    if (name == "ostkaka" || name == "gustav9797")
                                    {
                                        new Thread(() =>
                                            {
                                                Generate(OstBot.room.width, OstBot.room.height);//lock(OstBot.playerListLock
                                            }).Start();
                                    }
                                    break;

                                //case "!cheat":

                            }
                        }
                    }
                    break;

                case "m":
                    {
                        

                        new Thread(() =>
                            {
                                int userId = m.GetInt(0);
                                float playerPosX = m.GetFloat(1);
                                float playerPosY = m.GetFloat(2);
                                float speedX = m.GetFloat(3);
                                float speedY = m.GetFloat(4);
                                float modifierX = m.GetFloat(5);
                                float modifierY = m.GetFloat(6);
                                float horizontal = m.GetFloat(7);
                                float vertical = m.GetFloat(8);
                                int Coins = m.GetInt(9);
                                bool purple = m.GetBoolean(10);
                                bool hasLevitation = m.GetBoolean(11);

                                int blockX = (int)(playerPosX / 16 + 0.5) + (int)horizontal;
                                int blockY = (int)(playerPosY / 16 + 0.5) + (int)vertical;

                                BotPlayer player;

                                lock (OstBot.playerListLock)
                                {
                                    if (!OstBot.playerList.ContainsKey(userId))
                                        return;
                                    else
                                        player = OstBot.playerList[userId];
                                }
                                if (player.name == "ostkaka")
                                    Console.WriteLine(horizontal.ToString() + " " + vertical.ToString());

                                int blockId = (OstBot.room.getMapBlock(0, blockX + (int)horizontal, blockY + (int)vertical, 0).blockId);
                                if (blockId >= Skylight.BlockIds.Blocks.Sand.BROWN - 5 && blockId <= Skylight.BlockIds.Blocks.Sand.BROWN)
                                {

                                    if (player.digRange > 1)
                                    {
                                        for (int x = (horizontal == 1) ? -1 : -player.digRange + 1; x < ((horizontal == -1) ? 2 : player.digRange); x++)
                                        {
                                            for (int y = (vertical == 1) ? -1 : -player.digRange + 1; y < ((vertical == -1) ? 2 : player.digRange); y++)
                                            {
                                                Console.WriteLine("snor är :" + x.ToString() + "    och skit är: " + y.ToString());

                                                blockId = (OstBot.room.getMapBlock(0, blockX + x, blockY + y, 0).blockId);
                                                if (blockId >= Skylight.BlockIds.Blocks.Sand.BROWN - 5 && blockId <= Skylight.BlockIds.Blocks.Sand.BROWN)
                                                {
                                                    float distance = (float)Math.Sqrt(Math.Pow(x + horizontal, 2) + Math.Pow(y + vertical, 2));

                                                    if (distance < 1.4142 * (player.digRange - 1) || distance < 1.4142)
                                                        OstBot.room.DrawBlock(Block.CreateBlock(0, blockX + x, blockY + y, 4, -1));
                                                }
                                            }
                                        }
                                    }
                                    else
                                    {
                                        OstBot.room.DrawBlock(Block.CreateBlock(0, blockX, blockY, 4, -1));
                                        if (horizontal == 0 || vertical == 0)
                                            OstBot.room.DrawBlock(Block.CreateBlock(0, blockX + (int)horizontal, blockY + (int)vertical, 4, -1));
                                    }

                                }
                            }).Start();
                    }
                    break;

            }
        }

        public void onDisconnect(object sender, string reason)
        {

        }

        private int getDugBlockId(int blockId)
        {
            if (blockId >= Skylight.BlockIds.Blocks.Sand.BROWN - 5 && blockId <= Skylight.BlockIds.Blocks.Sand.BROWN)
                return 4;

            switch (blockId)
            {
                case Skylight.BlockIds.Blocks.Sand.BROWN:
                    return 4;
                default:
                    return blockId;
            }
        }
    }
}
