using Backend.Database;
using FrontEnd.ExtensionMethods;
using System.Windows;
using WpfApp1.model;
using TimeZone = WpfApp1.model.TimeZone;

namespace WpfApp1
{
    public partial class App : Application
    {
        public App()
        {
            //C:\\Users\\salva\\AppData\\Roaming\\WpfApp1\\mydb.db"
            //Sys.LoadAllEmbeddedDll(); //load some custom assemblies that could be used later on.
            DatabaseManager.DatabaseName = "data\\zodiac.db";
            //DatabaseManager.LoadInApplicationData();
            DatabaseManager.Add(new SQLiteDatabase<Sign>());
            DatabaseManager.Add(new SQLiteDatabase<Gender>());
            DatabaseManager.Add(new SQLiteDatabase<Triplicity>());
            DatabaseManager.Add(new SQLiteDatabase<Element>());
            DatabaseManager.Add(new SQLiteDatabase<Power>());
            DatabaseManager.Add(new SQLiteDatabase<StarPower>());
            DatabaseManager.Add(new SQLiteDatabase<Person>());
            DatabaseManager.Add(new SQLiteDatabase<Star>());
            DatabaseManager.Add(new SQLiteDatabase<House>());
            DatabaseManager.Add(new SQLiteDatabase<Aspect>());

            //ATLAS
            DatabaseManager.Add(new SQLiteDatabase<Country>());
            DatabaseManager.Add(new SQLiteDatabase<Region>());
            DatabaseManager.Add(new SQLiteDatabase<TimeZone>());
            DatabaseManager.Add(new SQLiteDatabase<City>());

            DatabaseManager.Add(new SQLiteDatabase<StarTransitOrbit>());
            DatabaseManager.Add(new SQLiteDatabase<TransitType>());
            DatabaseManager.Add(new SQLiteDatabase<Energy>());

            DatabaseManager.Add(new SQLiteDatabase<LibraryAspect>());
            this.DisposeOnExit(); // ensure Databases are disposed on Application' shutdown.
        }
    }
}