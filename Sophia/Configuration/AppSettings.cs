using System;
using System.Collections.Generic;
using System.Text;

namespace Configuration
{
    public class AppSettings
    {
        #region BlackList
        public static string DbConnectionString { get { return Environment.GetEnvironmentVariable("DbConnectionString"); } }
        //public static string DbConnectionStringPassword { get { return Environment.GetEnvironmentVariable("DbConnectionStringPassword"); } }

        //public static string DbConnectionStringDevel { get { return System.Environment.GetEnvironmentVariable("DbConnectionStringDevel"); } }
        //public static string DbConnectionStringOldBlackList { get { return System.Environment.GetEnvironmentVariable("DbConnectionStringOldBlackList"); } }

        public static string VolumePath { get { return Environment.GetEnvironmentVariable("VolumePath"); } }
        #endregion

        //#region MinJust
        //public static string MinJustLink { get { return System.Environment.GetEnvironmentVariable("MinJustLink"); } }
        //#endregion

        //#region PEP
        //public static string DbConnectionStringOracle { get { return System.Environment.GetEnvironmentVariable("DbConnectionStringOracle"); } }
        //public static string DbConnectionStringOraclePassword { get { return getSecret("DbConnectionStringOraclePassword"); } }
        //public static string PepLogin { get { return System.Environment.GetEnvironmentVariable("PepLogin"); } }
        //public static string PepPassword { get { return getSecret("PepPassword"); } }
        //public static string KisPepFile { get { return System.Environment.GetEnvironmentVariable("KisPepFile"); } }
        //public static string KisPovFile { get { return System.Environment.GetEnvironmentVariable("KisPovFile"); } }
        //public static string PepsFile { get { return System.Environment.GetEnvironmentVariable("PepsFile"); } }
        //public static string PepXmlLink { get { return System.Environment.GetEnvironmentVariable("PepXmlLink"); } }
        //public static string PepPovXmlLink { get { return System.Environment.GetEnvironmentVariable("PepPovXmlLink"); } }
        //public static string PepOpendataXmlLink { get { return System.Environment.GetEnvironmentVariable("PepOpendataXmlLink"); } }
        //public static string PepUploadFolder { get { return System.Environment.GetEnvironmentVariable("PepUploadFolder"); } }
        //public static string Environment { get { return System.Environment.GetEnvironmentVariable("Environment"); } }
        //#endregion

        //#region Green Theater
        //public static string GtAuthorizationUrl { get { return System.Environment.GetEnvironmentVariable("GtAuthorizationUrl"); } }
        //public static string GtContactsUrl { get { return System.Environment.GetEnvironmentVariable("GtContactsUrl"); } }
        //public static string GtCardServicesConnectionString { get { return System.Environment.GetEnvironmentVariable("GtCardServicesConnectionString"); } }
        //public static string GtCardServicesConnectionStringPassword { get { return getSecret("GtCardServicesConnectionStringPassword"); } }
        //public static string GtUserLogin { get { return System.Environment.GetEnvironmentVariable("GtUserLogin"); } }
        //public static string GtUserHash { get { return getSecret("GtUserHash"); } }
        //#endregion

        //#region Ema

        //public static string EmaUrl { get { return System.Environment.GetEnvironmentVariable("EmaUrl"); } }
        //public static string EmaLogin { get { return System.Environment.GetEnvironmentVariable("EmaLogin"); } }
        //public static string EmaPassword { get { return System.Environment.GetEnvironmentVariable("EmaPassword"); } }
        //public static string EmaChangePasswordCode { get { return System.Environment.GetEnvironmentVariable("EmaChangePasswordCode"); } }

        //#endregion

        //#region UbkiSend
        //public static string UbkiSendLogin { get { return System.Environment.GetEnvironmentVariable("UbkiSendLogin"); } }
        //public static string UbkiSendPassword { get { return System.Environment.GetEnvironmentVariable("UbkiSendPassword"); } }
        //public static string UbkiSendAuthLink { get { return System.Environment.GetEnvironmentVariable("UbkiSendAuthLink"); } }
        //public static string UbkiSendUploadLink { get { return System.Environment.GetEnvironmentVariable("UbkiSendUploadLink"); } }
        //public static string UbkiSendConnectionString { get { return System.Environment.GetEnvironmentVariable("UbkiSendConnectionString"); } }
        //public static string UbkiOracleConnectionString { get { return System.Environment.GetEnvironmentVariable("UbkiOracleConnectionString"); } }
        //#endregion

        //#region Pvbki
        //public static string PvbkiSendClientsPerPackage { get { return System.Environment.GetEnvironmentVariable("PvbkiSendClientsPerPackage"); } }
        //#endregion

        //#region GRODI

        //public static string CertFileName { get { return System.Environment.GetEnvironmentVariable("CertFileName"); } }
        //public static string CertPassword { get { return System.Environment.GetEnvironmentVariable("CertPassword"); } }

        //#endregion
    }
}
