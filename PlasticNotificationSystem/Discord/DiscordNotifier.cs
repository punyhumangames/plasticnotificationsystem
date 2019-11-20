using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;
using Discord.Webhook;
using Discord;
using System.Linq;
using System.ComponentModel;

namespace PlasticNotificationSystem.Discord
{
    class DiscordNotifierConfig
    {
       
        public string URL
        {
            get;
            set;
        }

        public bool ShowDetails
        {
            get;
            set;
        }

        public bool ShowAllFileChanges
        {
            get;
            set;
        }

        public bool ShowAuthor
        {
            get;
            set;
        }

        public bool ShowServer
        {
            get;
            set;
        }
       
        public string Name
        {
            get;
            set;
        }

        [DefaultValue(15)]
        public int NumDetails
        {
            get;
            set;
        }
           
    }

    class DiscordNotifier : INotifier
    {
        static NLog.Logger Logger = NLog.LogManager.GetCurrentClassLogger();

        IEnumerable<DiscordNotifierConfig> Config
        {
            get;
            set;
        }

        public void NotifyTrigger(ITriggerEvent Event)
        {
            foreach (DiscordNotifierConfig Webhook in Config)
            {
                EmbedBuilder EmbedBuilder = new EmbedBuilder()
                .WithTitle(Event.Title)
                .WithDescription(Event.Body)                
                .WithCurrentTimestamp();

                string Footer = "";

                if(Webhook.ShowAuthor)
                {  
                   Footer += "Author " + Event.Author;
                }

                if (Webhook.ShowServer)
                {
                    if (Footer != "")
                    {
                        Footer += " ";
                    }
                    Footer += "on Server: " + Event.Server;
                }
                if(Footer != "")
                {
                    EmbedBuilder.WithFooter(Footer);
                }

                if(Webhook.ShowDetails && Event is IWithDetails)
                {
                    IEnumerable<object> Details = (Event as IWithDetails).Details;

                    if(Details.FirstOrDefault() != null)
                    {
                        bool HasDirectories = true;

                        //Strip out directory changes if we are a FileChange and we don't want directory changes
                        if (Details.First() is IWithFileChange)
                        {
                            bool HasFiles = Details.Count(x => (x as IWithFileChange).FileType == FileType.File) > 0;
                            if (!Webhook.ShowAllFileChanges && HasFiles)
                            {
                                Details = Details.Where(x => (x as IWithFileChange).FileType == FileType.File);
                                HasDirectories = false;
                            }
                        }                       

                        Logger.Info("Appending {0} Details. Num Details is {1}", Details.Count(), Webhook.NumDetails);
                        if(Details.Count() > Webhook.NumDetails)
                        {
                            string AndDirectories = "";
                            if(HasDirectories)
                            {
                                AndDirectories = " and Directories";
                            }

                            EmbedBuilder.Description += string.Format("{1}{1}{1}Showing {2}/{0} Files{3}", Details.Count(), Environment.NewLine, Webhook.NumDetails, AndDirectories);
                            Details = Details.Take(Webhook.NumDetails);
                            Logger.Info("Too many details, limiting to {0} Details", Details.Count());
                        }

                        if (Details.First() is IWithFileChange)
                        {                           
                            EmbedFieldBuilder[] Fields = new EmbedFieldBuilder[]
                            {
                                new EmbedFieldBuilder().WithName("Operation")
                                    .WithValue(string.Join(Environment.NewLine, 
                                        Details.Select(x => (x as IWithFileChange).FileOperation.ToString())))
                                    .WithIsInline(true),
                                new EmbedFieldBuilder().WithName("File Name")
                                    .WithValue(string.Join(Environment.NewLine,
                                        Details.Select(x => (x as IWithFileChange).FileName.ToString())))
                                    .WithIsInline(true),
                                new EmbedFieldBuilder().WithName("Repository/Branch")
                                    .WithValue(string.Join(Environment.NewLine,
                                        Details.Select(x => (x as IWithFileChange).Repository+(x as IWithFileChange).Branch)))
                                    .WithIsInline(true),
                            };
                            EmbedBuilder.WithFields(Fields);
                        }
                        else
                        {
                            EmbedBuilder.WithFields(new EmbedFieldBuilder().WithName("Details").WithValue(string.Join(Environment.NewLine, Details.Select(x => x.ToString()))));
                        }
                        
                    }                   
                }
                
                DiscordWebhookClient c = new DiscordWebhookClient(Webhook.URL);
                c.SendMessageAsync(embeds: new Embed[] { EmbedBuilder.Build() }, username: Webhook.Name).Wait();
            }
        }

        public void ParseJsonString(string JsonString)
        {
            Config = JsonConvert.DeserializeObject<List<DiscordNotifierConfig>>(JsonString);
        }
    }
}
