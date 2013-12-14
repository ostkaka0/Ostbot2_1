using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using System.Drawing;

namespace OstBot_2_1
{
    public class Commands : BotSystem
    {
        private List<string> disabledPlayers;
        private List<string> protectedPlayers;
        private List<string> getPlacerPlayers;

        public Commands(OstBot ostBot)
            : base(ostBot)
        {
            enabled = true;
        }

        public override void onEnable()
        {
            disabledPlayers = new List<string>();
            protectedPlayers = new List<string>();
            getPlacerPlayers = new List<string>();
        }

        public override void onDisable()
        {
            disabledPlayers = null;
            protectedPlayers = null;
            getPlacerPlayers = null;
        }

        public override void onMessage(object sender, OstBot ostBot, PlayerIOClient.Message m)
        {
            Console.WriteLine(m.Type);
            switch (m.Type)
            {
                case "b":
                    new Task(() =>
                    {
                        string name = "";
                        if (m.Count >= 5)
                        {
                            lock (ostBot.playerList)
                            {
                                if (ostBot.playerList.ContainsKey(m.GetInt(4)))
                                {
                                    name = ostBot.playerList[m.GetInt(4)].Name;
                                }
                            }
                            if (name == "") // else if
                            {
                                lock (ostBot.leftPlayerList)
                                {
                                    if (ostBot.leftPlayerList.ContainsKey(m.GetInt(4)))
                                    {
                                        name = ostBot.leftPlayerList[m.GetInt(4)].Name;
                                    }
                                }
                            }
                        }

                        if (disabledPlayers.Contains(name))
                        {
                            /*return;
                        }*/
                            Block trollBlock = new Block(m, ostBot);
                            Block block;
                            for (int i = 0; true; i++)
                            {
                                block = ostBot.room.getMapBlock(trollBlock.layer, trollBlock.x, trollBlock.y, i);
                                /*string name2 = "";

                                lock (ostBot.playerList)
                                {
                                    if (ostBot.playerList.ContainsKey(block.placer.UserId))
                                    {
                                        name2 = ostBot.playerList[block.placer.UserId].Name;
                                    }
                                }
                                if (name == "") //else if
                                {
                                    lock (ostBot.leftPlayerList)
                                    {
                                        if (ostBot.leftPlayerList.ContainsKey(block.placer.UserId))
                                        {
                                            name2 = ostBot.leftPlayerList[block.placer.UserId].Name;
                                        }
                                    }
                                }*/
                                if (block.placer == null)
                                    break;

                                lock (disabledPlayers)
                                {
                                    if (!disabledPlayers.Contains(block.placer.Name))
                                        break;
                                }
                            }
                            ostBot.room.DrawBlock(block);
                        }
                        else if (!protectedPlayers.Contains(name))
                        {
                            Block trollBlock = new Block(m, ostBot);
                            Block oldBlock = ostBot.room.getMapBlock(trollBlock.layer, trollBlock.x, trollBlock.y, 1);

                            lock (ostBot.playerList)
                            {
                                if (ostBot.playerList.ContainsKey(oldBlock.placer.UserId))
                                {
                                    if (protectedPlayers.Contains(oldBlock.placer.Name))
                                    {
                                        ostBot.room.DrawBlock(oldBlock);
                                    }
                                }
                            }
                        }
                        if (getPlacerPlayers.Contains(name))
                        {
                            getPlacerPlayers.Remove(name);
                            string name2 = "[undefined]";
                            Block block = new Block(m, ostBot);
                            Block oldBlock = ostBot.room.getMapBlock(block.layer, block.x, block.y, 1);

                            lock (ostBot.playerList)
                            {
                                if (ostBot.playerList.ContainsKey(oldBlock.placer.UserId))
                                {
                                    name2 = oldBlock.placer.Name;
                                }
                            }
                            if (name2 == "[undefined]")
                            {
                                lock (ostBot.leftPlayerList)
                                {
                                    if (ostBot.leftPlayerList.ContainsKey(oldBlock.placer.UserId))
                                    {
                                        name2 = oldBlock.placer.Name;
                                    }
                                }
                            }

                            ostBot.connection.Send("say", name + ": Block placed by " + name2);
                        }
                    }).Start();
                    break;
            }
        }

        public override void onDisconnect(object sender, string reason)
        {

        }

        public override void onCommand(object sender, OstBot ostBot, string text, string[] args, int userId, Player player, string name, bool isBotMod)
        {


            switch (args[0])
            {
                case "woot":
                    if (isBotMod)
                        ostBot.connection.Send("wootup");
                    break;
                case "reset":
                    if (isBotMod)
                        ostBot.connection.Send("say", "/reset");
                    break;
                case "loadlevel":
                    if (isBotMod)
                        ostBot.connection.Send("say", "/loadlevel");
                    break;
                case "save":
                    if (isBotMod)
                        ostBot.connection.Send("save");
                    break;
                case "clear":
                    if (isBotMod)
                        ostBot.connection.Send("clear");
                    break;
                case "kick":
                    break;
                case "ban":
                    break;
                case "fill":       //<blocktyp><data> / <blocktyp><lager> / <blocktyp><pengar till pengardörr>..   //med arean mellan 2 block
                    new Task(() =>
                    {
                        if (args.Length > 1 && isBotMod)
                        {
                            int blockId;
                            Int32.TryParse(args[1], out blockId);
                            int layer = (blockId >= 500) ? 1 : 0;
                            for (int y = 1; y < ostBot.room.height - 1; y++)
                            {
                                for (int x = 1; x < ostBot.room.width - 1; x++)
                                {
                                    ostBot.room.DrawBlock(Block.CreateBlock(layer, x, y, blockId, null));
                                }
                            }
                        }
                    }).Start();
                    break;
                case "fillworld":  //<blocktyp><data>
                    break;
                case "fillarea":   //<x1><y1><x2><y2><blocktyp><data>
                    break;
                case "fillexpand":
                    {
                        int toReplace = 0;
                        int toReplaceWith = 0;
                        if (args.Length == 2)
                        {
                            if (!int.TryParse(args[1], out toReplaceWith))
                            {
                                ostBot.connection.Send("say", "Usage: !fillexpand <from id=0> <to id>");
                                return;
                            }
                        }
                        else if (args.Length == 3)
                        {
                            if (!int.TryParse(args[2], out toReplaceWith) || !int.TryParse(args[1], out toReplace))
                            {
                                ostBot.connection.Send("say", "Usage: !fillexpand <from id=0> <to id>");
                                return;
                            }
                        }
                        Block startBlock = ostBot.room.getBotMapBlock(0, /*_*//*player.blockX, player.blockY*/0,0);
                        if (startBlock.blockId == toReplace)
                        {
                            int total = 0;
                            List<Point> closeBlocks = new List<Point> { new Point(1, 0), new Point(-1, 0), new Point(0, 1), new Point(0, -1) };
                            Queue<Point> blocksToCheck = new Queue<Point>();
                            List<Point> blocksToFill = new List<Point>();
                            blocksToCheck.Enqueue(new Point(startBlock.x, startBlock.y));
                            while (blocksToCheck.Count > 0)
                            {
                                Point parent = blocksToCheck.Dequeue();
                                //if (!blocksToFill.Contains(parent))
                                    for (int i = 0; i < closeBlocks.Count; i++)
                                    {
                                        Point current = new Point(closeBlocks[i].X + parent.X, closeBlocks[i].Y + parent.Y);
                                        Block currentBlock = ostBot.room.getBotMapBlock(0, current.X, current.Y);
                                        if (currentBlock.blockId == toReplace && !blocksToCheck.Contains(current) && !blocksToFill.Contains(current) && current.X >= 0 && current.Y >= 0 && current.X <= ostBot.room.width && current.Y <= ostBot.room.height)
                                        {
                                            blocksToFill.Add(current);
                                            blocksToCheck.Enqueue(current);
                                            total++;
                                            if (total > 10000)
                                            {
                                                ostBot.connection.Send("say", "Don't try to fill the whole world, fool!");
                                                return;
                                            }
                                        }
                                    }
                            }
                            ostBot.connection.Send("say", "total blocks: " + total + ". Filling..");
                            foreach (Point p in blocksToFill)
                            {
                                ostBot.room.DrawBlock(Block.CreateBlock(0, (int)p.X, (int)p.Y, toReplaceWith, player));
                            }
                        }
                    }
                    break;
                case "replace":        //med arean mellan 2 block
                    new Task(() =>
                    {
                        if (args.Length > 2 && isBotMod)
                        {
                            int blockId1, blockId2;
                            Int32.TryParse(args[1], out blockId1);
                            Int32.TryParse(args[2], out blockId2);
                            int layer1 = (blockId1 >= 500) ? 1 : 0;
                            int layer2 = (blockId2 >= 500) ? 1 : 0;
                            for (int y = 1; y < ostBot.room.height - 1; y++)
                            {
                                for (int x = 1; x < ostBot.room.width - 1; x++)
                                {
                                    if (ostBot.room.getBotMapBlock(layer1, x, y).blockId == blockId1)
                                        ostBot.room.DrawBlock(Block.CreateBlock(layer2, x, y, blockId2, null));
                                }
                            }
                        }
                    }).Start();
                    break;
                case "replaceworld":
                    break;
                case "replacearea":
                    break;

                case "getplacer":
                    if (!getPlacerPlayers.Contains(name))
                        getPlacerPlayers.Add(name);
                    break;
                case "protect":         //<name>
                    if (args.Length > 1 && isBotMod)
                    {
                        if (!protectedPlayers.Contains(args[1]))
                            protectedPlayers.Add(args[1]);
                    }
                    break;
                case "unprotect":         //<name>
                    if (args.Length > 1 && isBotMod)
                    {
                        if (protectedPlayers.Contains(args[1]))
                            protectedPlayers.Remove(args[1]);
                    }
                    break;
                case "repair":          //<name>
                    if (args.Length > 1 && isBotMod)
                    {
                        for (int l = 0; l < 2; l++)
                        {
                            for (int y = 0; y < ostBot.room.height; y++)
                            {
                                for (int x = 0; x < ostBot.room.width; x++)
                                {
                                    Block block;

                                    //if (ostBot.room.getBotMapBlock(l, x, y).b_userId == -1)
                                    //    continue;


                                    for (int i = 0; true; i++)
                                    {
                                        block = ostBot.room.getMapBlock(l, x, y, i);
                                        string userName = "";
                                        lock (ostBot.playerList)
                                        {
                                            if (ostBot.playerList.ContainsKey(block.placer.UserId))
                                            {
                                                userName = block.placer.Name;
                                            }
                                        }
                                        if (userName == "") //else if
                                        {
                                            lock (ostBot.leftPlayerList)
                                            {
                                                if (ostBot.leftPlayerList.ContainsKey(block.placer.UserId))
                                                {
                                                    userName = block.placer.Name;
                                                }
                                            }
                                        }

                                        //if (protectedPlayers.Contains(userName))
                                        //    break;

                                        if (userName == args[1])
                                        {
                                            ostBot.room.DrawBlock(block);
                                            break;
                                        }
                                        else
                                        {
                                            //Console.WriteLine("block borttaget från" + userName);
                                        }


                                    }
                                }
                            }
                        }
                    }
                    break;
                case "repairprotected":          //<name>
                    if (args.Length > 1 && isBotMod)
                    {
                        for (int l = 0; l < 2; l++)
                        {
                            for (int y = 0; y < ostBot.room.height; y++)
                            {
                                for (int x = 0; x < ostBot.room.width; x++)
                                {
                                    Block block;

                                    //if (ostBot.room.getBotMapBlock(l, x, y).b_userId == -1)
                                    //    continue;


                                    for (int i = 0; true; i++)
                                    {
                                        block = ostBot.room.getMapBlock(l, x, y, i);
                                        string userName = "";
                                        lock (ostBot.playerList)
                                        {
                                            if (ostBot.playerList.ContainsKey(block.placer.UserId))
                                            {
                                                userName = block.placer.Name;
                                            }
                                        }
                                        if (userName == "") //else if
                                        {
                                            lock (ostBot.leftPlayerList)
                                            {
                                                if (ostBot.leftPlayerList.ContainsKey(block.placer.UserId))
                                                {
                                                    userName = block.placer.Name;
                                                }
                                            }
                                        }

                                        //
                                        //    break;

                                        if (protectedPlayers.Contains(userName))//if (userName == args[1])
                                        {
                                            ostBot.room.DrawBlock(block);
                                            break;
                                        }
                                        else
                                        {
                                            //Console.WriteLine("block borttaget från" + userName);
                                        }


                                    }
                                }
                            }
                        }
                    }
                    break;
                case "disableedit":    //<spelarnamn>
                    if (args.Length > 1 && isBotMod)
                    {
                        lock (disabledPlayers)
                        {
                            if (!disabledPlayers.Contains(args[1]))
                                disabledPlayers.Add(args[1]);
                        }
                    }
                    break;
                case "enableedit":    //<spelarnamn>
                    if (args.Length > 1 && isBotMod)
                    {
                        lock (disabledPlayers)
                        {
                            if (disabledPlayers.Contains(args[1]))
                                disabledPlayers.Remove(args[1]);
                        }
                    }
                    break;
                case "rollback":   //<spelarnamn>
                    if (args.Length > 1 && isBotMod)
                    {
                        for (int l = 0; l < 2; l++)
                        {
                            for (int y = 0; y < ostBot.room.height; y++)
                            {
                                for (int x = 0; x < ostBot.room.width; x++)
                                {
                                    Block block;

                                    {
                                        Block block2 = ostBot.room.getBotMapBlock(l, x, y);
                                    
                                        if (block2.placer != null)
                                            if (block2.placer.UserId == -1)
                                                continue;
                                    }


                                    for (int i = 0; true; i++)
                                    {
                                        block = ostBot.room.getMapBlock(l, x, y, i);
                                        string userName = "";
                                        if (block.placer != null)
                                        {
                                            userName = block.placer.Name;
                                        }
                                        /*lock (ostBot.playerList)
                                        {
                                            if (ostBot.playerList.ContainsKey(block.placer.UserId))
                                            {
                                                userName = block.placer.Name;
                                            }
                                        }
                                        if (userName == "") //else if
                                        {
                                            lock (ostBot.leftPlayerList)
                                            {
                                                if (ostBot.leftPlayerList.ContainsKey(block.placer.UserId))
                                                {
                                                    userName = block.placer.Name;
                                                }
                                            }
                                        }*/

                                        if (userName != args[1])
                                        {
                                            ostBot.room.DrawBlock(block);
                                            break;
                                        }
                                        else
                                        {
                                            //Console.WriteLine("block borttaget från" + userName);
                                        }


                                    }
                                }
                            }
                        }
                    }
                    break;

                case "votedisable":
                    if (args.Length > 1)
                    {
                        int playerId;
                        lock (ostBot.nameList)
                        {
                            if (ostBot.nameList.ContainsKey(args[1]))
                            {
                                playerId = ostBot.nameList[args[1]];
                            }
                            else
                            {
                                break;
                            }
                        }
                        Player playerToDisable;
                        lock (ostBot.playerList)
                        {
                            if (ostBot.playerList.ContainsKey(playerId))
                            {
                                playerToDisable = ostBot.playerList[playerId];
                            }
                            else
                            {
                                break;
                            }
                        }
                    }
                    break;
            }
        }

        public override void Update()
        {

        }
    }
}
