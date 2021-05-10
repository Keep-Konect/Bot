using Discord;
using Discord.Audio;
using Discord.Commands;
using Discord.WebSocket;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Bot_Discord
{
    class Program
    {

        private DiscordSocketClient _client;
        private CommandService _commands;
        public static void Main(string[] args)
        => new Program().MainAsync().GetAwaiter().GetResult();

        public async Task MainAsync()
        {
            _client = new DiscordSocketClient();
            _commands = new CommandService(new CommandServiceConfig
            {
                LogLevel = LogSeverity.Info,
                CaseSensitiveCommands = false,
            });
            _commands.Log += Log;
            _client.Log += Log;

            var token = "ODI4OTE5NjExMjIyMzkyODMy.YGwliw.R7JOGm2PMySzPvknXpUS2z4cePY";
            await _client.LoginAsync(TokenType.Bot, token);
            await _client.StartAsync();
            _client.MessageReceived += ReceveurMessage;

            // Block this task until the program is closed.
            await Task.Delay(-1);
        }
        private Task Log(LogMessage msg)
        {
            Console.WriteLine(msg.ToString());
            return Task.CompletedTask;
        }
        private async Task ReceveurMessage(SocketMessage arg)
        {
            var msg = arg as SocketUserMessage;
            if (msg == null) return;
            if (msg.Author.Id == _client.CurrentUser.Id || msg.Author.IsBot) return;
            int position_dans_message = 0;
            if (msg.HasCharPrefix('!', ref position_dans_message))
            {
                string command = msg.ToString().Substring(1);
                switch (command)
                {
                    case "coucou":
                        await msg.Channel.SendMessageAsync("Bonjour " + msg.Author.Mention);
                        break;
                    case "aurevoir":
                        await msg.Channel.SendMessageAsync("Au revoir " + msg.Author.Mention);
                        await msg.Channel.SendFileAsync("C:\\Users\\Magic Makers\\Downloads\\1f44fec599a3eb517280c87ffdcb1ab3.png");
                        break;
                    case "play": 
                        await JoinChannel(msg);
                        break;
                    default:
                        await MessageAvecVariable(msg);
                        //await msg.Channel.SendMessageAsync("Unknow command");
                        break; 

                }
            }

        }
        private async Task MessageAvecVariable(SocketUserMessage arg)
        {
            string command = arg.ToString().Substring(1);
            string slap = "slap";
            if (command.Contains(slap) && arg.MentionedUsers.Count() == 1)
            {
                EmbedBuilder builder = new EmbedBuilder();
                builder.WithImageUrl("https://media1.tenor.com/images/31f29b3fcc20a486f44454209914266a/tenor.gif?itemid=17942299");
                await arg.Channel.SendMessageAsync("Slapped -> " + arg.MentionedUsers.FirstOrDefault().Mention, false, builder.Build());
            }

        }

        private async Task MusicClient(SocketUser user, SocketVoiceState state1, SocketVoiceState state2)
        {
            if (user.IsBot)
                return;

            if (state1.VoiceChannel == null && state2.VoiceChannel != null)
            {

            }
            else
            {
                return;
            }

            var connection = await state2.VoiceChannel.ConnectAsync();
            var voice = connection.CreatePCMStream(AudioApplication.Voice);

            string Filename = @"D:\Data\users\Konect\Music\C2C.webm";
            var psi = new ProcessStartInfo
            {
                FileName = "ffmpeg",
                Arguments = $@"-i ""{Filename}"" -ac 2 -f s16le -ar 48000 pipe:1",
                RedirectStandardOutput = true,
                UseShellExecute = false
            };
            var ffmpeg = Process.Start(psi);

            var output = ffmpeg.StandardOutput.BaseStream;

            await output.CopyToAsync(voice);
            await voice.FlushAsync();
        }
        public Task JoinChannel(SocketUserMessage arg, IVoiceChannel channel = null)
        {
            _ = Task.Run(async () =>
            {
                channel = channel ?? (arg.Author as IGuildUser)?.VoiceChannel;

                if (channel == null)
                {
                    await arg.Channel.SendMessageAsync("You need to be in a voice channel, or pass one as an argument.");
                    return;
                }

                var audioclient = await channel.ConnectAsync();
                var voice = audioclient.CreatePCMStream(AudioApplication.Voice);

                string Filename = @"D:\Data\users\Konect\Music\C2C.webm";
                var psi = new ProcessStartInfo
                {
                    FileName = "ffmpeg",
                    Arguments = $@"-i ""{Filename}"" -ac 2 -f s16le -ar 48000 pipe:1",
                    RedirectStandardOutput = true,
                    UseShellExecute = false
                };
                var ffmpeg = Process.Start(psi);

                var output = ffmpeg.StandardOutput.BaseStream;

                await output.CopyToAsync(voice);
                await voice.FlushAsync();
            });
            return Task.CompletedTask;
        }
    }
}
