using DataProvider.DataLayer;
using DataProvider.Models.AbstractLayer.AbstractFactory;
using DataProvider.Models.ConcreteLayer.ConcreteFactories;
//using Logging;
using DataProvider.Infrastructure.Extantions;
using NLog;
using System;
using System.Threading.Tasks;

namespace DataProvider.Models.ConcreteLayer
{
    public class FactoryMethod
    {
        private static Logger LoggerStatic { get; } = LogManager.GetCurrentClassLogger();

        public static async Task<AbstractFactory> GetFactory(int sourceId)
        {
            LoggerStatic.AddLogStart("FactoryMethod -> GetFactory: ");
            AbstractFactory factory;

            var sourceName = await DbAdapter.GetSourceName(sourceId);
            LoggerStatic.AddLogStart($"sourceName: " + sourceName.ToString());

            switch (sourceName)
            {
                case "Notary":
                    factory = new NotaryFactory();
                    LoggerStatic.AddLogStart($"factory2: " + factory.ToString());
                    break;
                //case "CourtExpert":
                //    factory = new CourtExpertFactory();
                //    break;
                //case "AppriserRegistry":
                //    factory = new AppraiserFactory();
                //    break;
                //case "AuditorRegistry":
                //    factory = new AuditorFactory();
                //    break;
                //case "TemporaryAdmin":
                //    factory = new TemporaryAdminFactory();
                //    break;
                //case "ArbitrationManager":
                //    factory = new ArbitrationManagerFactory();
                //    break;
                //case "LostPassport":
                //    factory = new PassportLostFactory();
                //    break;
                //case "LostPassportForTravelAbroad":
                //    factory = new PassportLostAbroadFactory();
                //    break;
                //case "InvalidatedPassport":
                //    factory = new PassportInvalidateFactory();
                //    break;
                //case "InvalidatedPassportForTravelAbroad":
                //    factory = new PassportInvalidateAbroadFactory();
                //    break;
                //case "MissingPeople":
                //    factory = new MissingPersonFactory();
                //    break;
                //case "LustrationPeople":
                //    factory = new LustrationPersonFactory();
                //    break;
                //case "WantedPeople":
                //    factory = new WantedPersonFactory();
                //    break;
                //case "CorruptPeople":
                //    factory = new CorruptPersonFactory();
                //    break;
                default:
                    throw new Exception($"Unsupport factory for sourceId {sourceId}");
            }

            return factory;
        }
    }
}
