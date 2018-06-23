using System;
using System.Threading.Tasks;
using Discord;
using Discord.Commands;
using MySql.Data.MySqlClient;

public class Commands : ModuleBase
{
    [Command("ping"), Summary("Test command")]
    public async Task Ping()
    {
        await ReplyAsync("Pong!");
    }

    [Command("record"), Summary("Checks records on Slidy's Server")]
    [Alias("wr")]
    public async Task GetRecord([Summary("Map name")] string map, [Summary("Style name")] string styleName = null)
    {
        Style style = Style.Normal;
        if(styleName != null)
        {
            style = SlidyRecord.GetStyleFromName(styleName);
        }

        if(style == Style.Invalid)
        {
            await ReplyAsync("Invalid style name");
            return;
        }

        try
        {
            MySqlParameter mapParam = new MySqlParameter();
            mapParam.ParameterName = "@map";
            mapParam.Value = "%" + map + "%";

            var query = "SELECT p.lastname, p.steamaccountid, r.timestamp, r.attempts, r.time, r.jumps, r.strafes, r.sync, r.strafetime, r.ssj, r.track, r.style "
                        + "FROM `t_records` r "
                        + "JOIN `t_players` p ON p.playerid = r.playerid "
                        + "JOIN `t_maps` m ON m.mapid = r.mapid "
                        + "WHERE m.mapname LIKE @map AND r.track = 0 AND r.style = '" + ((int)style).ToString() + "' "
                        + "ORDER BY r.time ASC LIMIT 1";

            using (var command = Sql.Query(query))
            {
                command.CommandTimeout = 60;
                command.Parameters.Add(mapParam);

                using (var reader = await command.ExecuteReaderAsync() as MySqlDataReader)
                {
                    if (reader.Read())
                    {
                        SlidyRecord record = new SlidyRecord(reader);
                        await ReplyAsync("", false, await record.GetEmbed(map));
                    }
                    else
                    {
                        await ReplyAsync("No record found");
                    }
                }
            }
        }
        catch(MySqlException exception)
        {
            await ReplyAsync(exception.InnerException.Message);
        }
        catch(Exception exception)
        {

            await ReplyAsync(exception.InnerException.Message);
        }
    }

    [Command("precord"), Summary("Checks records on Slidy's Server for a specific player")]
    [Alias("pr", "pb")]
    public async Task GetPlayerRecord([Summary("Map name")] string map, [Summary("Player name")] string player, [Summary("Style name")] string styleName = null)
    {
        Style style = Style.Normal;
        if (styleName != null)
        {
            style = SlidyRecord.GetStyleFromName(styleName);
        }

        if (style == Style.Invalid)
        {
            await ReplyAsync("Invalid style name");
            return;
        }

        try
        {
            MySqlParameter mapParam = new MySqlParameter();
            mapParam.ParameterName = "@map";
            mapParam.Value = "%" + map + "%";
            MySqlParameter nameParam = new MySqlParameter();
            nameParam.ParameterName = "@name";
            nameParam.Value = "%" + player + "%";

            var query = "SELECT p.lastname, p.steamaccountid, r.timestamp, r.attempts, r.time, r.jumps, r.strafes, r.sync, r.strafetime, r.ssj, r.track, r.style "
                        + "FROM `t_records` r "
                        + "JOIN `t_players` p ON p.playerid = r.playerid "
                        + "JOIN `t_maps` m ON m.mapid = r.mapid "
                        + "WHERE m.mapname LIKE @map AND p.lastname LIKE @name AND r.track = '0' AND r.style = '" + ((int)style).ToString() + "' "
                        + "ORDER BY r.time ASC LIMIT 1";

            using (var command = Sql.Query(query))
            {
                command.CommandTimeout = 60;
                command.Parameters.Add(mapParam);
                command.Parameters.Add(nameParam);

                using (var reader = await command.ExecuteReaderAsync() as MySqlDataReader)
                {
                    if (reader.Read())
                    {
                        SlidyRecord record = new SlidyRecord(reader);
                        await ReplyAsync("", false, await record.GetEmbed(map));
                    }
                    else
                    {
                        await ReplyAsync("No record found");
                    }
                }
            }
        }
        catch (MySqlException exception)
        {
            await ReplyAsync(exception.InnerException.Message);
        }
        catch (Exception exception)
        {

            await ReplyAsync(exception.InnerException.Message);
        }
    }
}
