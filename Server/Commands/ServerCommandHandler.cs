using betareborn.Entities;
using betareborn.Items;
using betareborn.Network.Packets.Play;
using betareborn.Util;
using betareborn.Worlds;
using java.lang;
using java.util.logging;

namespace betareborn.Server.Commands
{
    public class ServerCommandHandler
    {
        private static Logger logger = Logger.getLogger("Minecraft");
        private readonly MinecraftServer server;

        public ServerCommandHandler(MinecraftServer server)
        {
            this.server = server;
        }

        public void executeCommand(Command command)
        {
            string var2 = command.commandAndArgs;
            CommandOutput var3 = command.output;
            string var4 = var3.getName();
            PlayerManager var5 = server.playerManager;
            if (var2.ToLower().StartsWith("help") || var2.ToLower().StartsWith("?"))
            {
                displayHelp(var3);
            }
            else if (var2.ToLower().StartsWith("list"))
            {
                var3.sendMessage("Connected players: " + var5.getPlayerList());
            }
            else if (var2.ToLower().StartsWith("stop"))
            {
                logCommand(var4, "Stopping the server..");
                server.stop();
            }
            else if (var2.ToLower().StartsWith("save-all"))
            {
                logCommand(var4, "Forcing save..");
                if (var5 != null)
                {
                    var5.savePlayers();
                }

                for (int var6 = 0; var6 < server.worlds.Length; var6++)
                {
                    ServerWorld var7 = server.worlds[var6];
                    var7.saveWithLoadingDisplay(true, null);
                }

                logCommand(var4, "Save complete.");
            }
            else if (var2.ToLower().StartsWith("save-off"))
            {
                logCommand(var4, "Disabling level saving..");

                for (int var17 = 0; var17 < server.worlds.Length; var17++)
                {
                    ServerWorld var30 = server.worlds[var17];
                    var30.savingDisabled = true;
                }
            }
            else if (var2.ToLower().StartsWith("save-on"))
            {
                logCommand(var4, "Enabling level saving..");

                for (int var18 = 0; var18 < server.worlds.Length; var18++)
                {
                    ServerWorld var31 = server.worlds[var18];
                    var31.savingDisabled = false;
                }
            }
            else if (var2.ToLower().StartsWith("op "))
            {
                string var19 = var2.Substring(var2.IndexOf(" ")).Trim();
                var5.addToOperators(var19);
                logCommand(var4, "Opping " + var19);
                var5.messagePlayer(var19, "§eYou are now op!");
            }
            else if (var2.ToLower().StartsWith("deop "))
            {
                string var20 = var2.Substring(var2.IndexOf(" ")).Trim();
                var5.removeFromOperators(var20);
                var5.messagePlayer(var20, "§eYou are no longer op!");
                logCommand(var4, "De-opping " + var20);
            }
            else if (var2.ToLower().StartsWith("ban-ip "))
            {
                string var21 = var2.Substring(var2.IndexOf(" ")).Trim();
                var5.banIp(var21);
                logCommand(var4, "Banning ip " + var21);
            }
            else if (var2.ToLower().StartsWith("pardon-ip "))
            {
                string var22 = var2.Substring(var2.IndexOf(" ")).Trim();
                var5.unbanIp(var22);
                logCommand(var4, "Pardoning ip " + var22);
            }
            else if (var2.ToLower().StartsWith("ban "))
            {
                string var23 = var2.Substring(var2.IndexOf(" ")).Trim();
                var5.banPlayer(var23);
                logCommand(var4, "Banning " + var23);
                ServerPlayerEntity var32 = var5.getPlayer(var23);
                if (var32 != null)
                {
                    var32.networkHandler.disconnect("Banned by admin");
                }
            }
            else if (var2.ToLower().StartsWith("pardon "))
            {
                string var24 = var2.Substring(var2.IndexOf(" ")).Trim();
                var5.unbanPlayer(var24);
                logCommand(var4, "Pardoning " + var24);
            }
            else if (var2.ToLower().StartsWith("kick "))
            {
                string var25 = var2.Substring(var2.IndexOf(" ")).Trim();
                ServerPlayerEntity var33 = null;

                for (int var8 = 0; var8 < var5.players.size(); var8++)
                {
                    ServerPlayerEntity var9 = (ServerPlayerEntity)var5.players.get(var8);
                    if (var9.name.EqualsIgnoreCase(var25))
                    {
                        var33 = var9;
                    }
                }

                if (var33 != null)
                {
                    var33.networkHandler.disconnect("Kicked by admin");
                    logCommand(var4, "Kicking " + var33.name);
                }
                else
                {
                    var3.sendMessage("Can't find user " + var25 + ". No kick.");
                }
            }
            else if (var2.ToLower().StartsWith("tp "))
            {
                string[] var26 = var2.Split(" ");
                if (var26.Length == 3)
                {
                    ServerPlayerEntity var34 = var5.getPlayer(var26[1]);
                    ServerPlayerEntity var37 = var5.getPlayer(var26[2]);
                    if (var34 == null)
                    {
                        var3.sendMessage("Can't find user " + var26[1] + ". No tp.");
                    }
                    else if (var37 == null)
                    {
                        var3.sendMessage("Can't find user " + var26[2] + ". No tp.");
                    }
                    else if (var34.dimensionId != var37.dimensionId)
                    {
                        var3.sendMessage("User " + var26[1] + " and " + var26[2] + " are in different dimensions. No tp.");
                    }
                    else
                    {
                        var34.networkHandler.teleport(var37.x, var37.y, var37.z, var37.yaw, var37.pitch);
                        logCommand(var4, "Teleporting " + var26[1] + " to " + var26[2] + ".");
                    }
                }
                else
                {
                    var3.sendMessage("Syntax error, please provice a source and a target.");
                }
            }
            else if (var2.ToLower().StartsWith("give "))
            {
                string[] var27 = var2.Split(" ");
                if (var27.Length != 3 && var27.Length != 4)
                {
                    return;
                }

                string var35 = var27[1];
                ServerPlayerEntity var38 = var5.getPlayer(var35);
                if (var38 != null)
                {
                    try
                    {
                        int var40 = Integer.parseInt(var27[2]);
                        if (Item.ITEMS[var40] != null)
                        {
                            logCommand(var4, "Giving " + var38.name + " some " + var40);
                            int var10 = 1;
                            if (var27.Length > 3)
                            {
                                var10 = parseInt(var27[3], 1);
                            }

                            if (var10 < 1)
                            {
                                var10 = 1;
                            }

                            if (var10 > 64)
                            {
                                var10 = 64;
                            }

                            var38.dropItem(new ItemStack(var40, var10, 0));
                        }
                        else
                        {
                            var3.sendMessage("There's no item with id " + var40);
                        }
                    }
                    catch (NumberFormatException)
                    {
                        var3.sendMessage("There's no item with id " + var27[2]);
                    }
                }
                else
                {
                    var3.sendMessage("Can't find user " + var35);
                }
            }
            else if (var2.ToLower().StartsWith("time "))
            {
                string[] var28 = var2.Split(" ");
                if (var28.Length != 3)
                {
                    return;
                }

                string var36 = var28[1];

                try
                {
                    int var39 = Integer.parseInt(var28[2]);
                    if ("add".EqualsIgnoreCase(var36))
                    {
                        for (int var41 = 0; var41 < server.worlds.Length; var41++)
                        {
                            ServerWorld var43 = server.worlds[var41];
                            var43.synchronizeTimeAndUpdates(var43.getTime() + var39);
                        }

                        logCommand(var4, "Added " + var39 + " to time");
                    }
                    else if ("set".EqualsIgnoreCase(var36))
                    {
                        for (int var42 = 0; var42 < server.worlds.Length; var42++)
                        {
                            ServerWorld var44 = server.worlds[var42];
                            var44.synchronizeTimeAndUpdates(var39);
                        }

                        logCommand(var4, "Set time to " + var39);
                    }
                    else
                    {
                        var3.sendMessage("Unknown method, use either \"add\" or \"set\"");
                    }
                }
                catch (NumberFormatException)
                {
                    var3.sendMessage("Unable to convert time value, " + var28[2]);
                }
            }
            else if (var2.ToLower().StartsWith("say "))
            {
                var2 = var2.Substring(var2.IndexOf(" ")).Trim();
                logger.info("[" + var4 + "] " + var2);
                var5.sendToAll(new ChatMessagePacket("§d[Server] " + var2));
            }
            else if (var2.ToLower().StartsWith("tell "))
            {
                string[] var29 = var2.Split(" ");
                if (var29.Length >= 3)
                {
                    var2 = var2.Substring(var2.IndexOf(" ")).Trim();
                    var2 = var2.Substring(var2.IndexOf(" ")).Trim();
                    logger.info("[" + var4 + "->" + var29[1] + "] " + var2);
                    var2 = "§7" + var4 + " whispers " + var2;
                    logger.info(var2);
                    if (!var5.sendPacket(var29[1], new ChatMessagePacket(var2)))
                    {
                        var3.sendMessage("There's no player by that name online.");
                    }
                }
            }
            else if (var2.ToLower().StartsWith("whitelist "))
            {
                executeWhitelist(var4, var2, var3);
            }
            else
            {
                logger.info("Unknown console command. Type \"help\" for help.");
            }
        }

        private void executeWhitelist(string commandUser, string message, CommandOutput output)
        {
            string[] var4 = message.Split(" ");
            if (var4.Length >= 2)
            {
                string var5 = var4[1].ToLower();
                if ("on".Equals(var5))
                {
                    logCommand(commandUser, "Turned on white-listing");
                    server.properties.setProperty("white-list", true);
                }
                else if ("off".Equals(var5))
                {
                    logCommand(commandUser, "Turned off white-listing");
                    server.properties.setProperty("white-list", false);
                }
                else if ("list".Equals(var5))
                {
                    var var6 = server.playerManager.getWhitelist();
                    string var7 = "";

                    foreach (string var9 in var6)
                    {
                        var7 = var7 + var9 + " ";
                    }

                    output.sendMessage("White-listed players: " + var7);
                }
                else if ("add".Equals(var5) && var4.Length == 3)
                {
                    string var11 = var4[2].ToLower();
                    server.playerManager.addToWhitelist(var11);
                    logCommand(commandUser, "Added " + var11 + " to white-list");
                }
                else if ("remove".Equals(var5) && var4.Length == 3)
                {
                    string var10 = var4[2].ToLower();
                    server.playerManager.removeFromWhitelist(var10);
                    logCommand(commandUser, "Removed " + var10 + " from white-list");
                }
                else if ("reload".Equals(var5))
                {
                    server.playerManager.reloadWhitelist();
                    logCommand(commandUser, "Reloaded white-list from file");
                }
            }
        }

        private static void displayHelp(CommandOutput output)
        {
            output.sendMessage("To run the server without a gui, start it like this:");
            output.sendMessage("   java -Xmx1024M -Xms1024M -jar minecraft_server.jar nogui");
            output.sendMessage("Console commands:");
            output.sendMessage("   help  or  ?               shows this message");
            output.sendMessage("   kick <player>             removes a player from the server");
            output.sendMessage("   ban <player>              bans a player from the server");
            output.sendMessage("   pardon <player>           pardons a banned player so that they can connect again");
            output.sendMessage("   ban-ip <ip>               bans an IP address from the server");
            output.sendMessage("   pardon-ip <ip>            pardons a banned IP address so that they can connect again");
            output.sendMessage("   op <player>               turns a player into an op");
            output.sendMessage("   deop <player>             removes op status from a player");
            output.sendMessage("   tp <player1> <player2>    moves one player to the same location as another player");
            output.sendMessage("   give <player> <id> [num]  gives a player a resource");
            output.sendMessage("   tell <player> <message>   sends a private message to a player");
            output.sendMessage("   stop                      gracefully stops the server");
            output.sendMessage("   save-all                  forces a server-wide level save");
            output.sendMessage("   save-off                  disables terrain saving (useful for backup scripts)");
            output.sendMessage("   save-on                   re-enables terrain saving");
            output.sendMessage("   list                      lists all currently connected players");
            output.sendMessage("   say <message>             broadcasts a message to all players");
            output.sendMessage("   time <add|set> <amount>   adds to or sets the world time (0-24000)");
        }

        private void logCommand(string commandUser, string message)
        {
            string var3 = commandUser + ": " + message;
            server.playerManager.broadcast("§7(" + var3 + ")");
            logger.info(var3);
        }

        private static int parseInt(string strin, int fallback)
        {
            try
            {
                return Integer.parseInt(strin);
            }
            catch (NumberFormatException)
            {
                return fallback;
            }
        }
    }
}
