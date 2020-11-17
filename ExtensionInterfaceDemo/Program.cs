using System;
using System.Collections.Generic;
using System.Dynamic;

namespace ExtensionInterfaceDemo
{
    public class Program
    {
        public static void Main(string[] args)
        {
            // Register car factories with factory finder
            CarInstaller.install();
            
            // Access factory finder
            var finder = FactoryFinder.getInstance();

            try
            {
                IFactory factory = null;
                IDiesel dCar = null;
                IElectric eCar = null;
                IHybrid hCar = null;
                
                factory = finder.findFactory(InterfaceID.ID_Diesel);
                dCar = (IDiesel) factory.create();
                dCar.fuel();
                
                factory = finder.findFactory(InterfaceID.ID_Electric);
                eCar = (IElectric) factory.create();
                eCar.charge();

                factory = finder.findFactory(InterfaceID.ID_Hybrid);
                dCar = (IDiesel) factory.create();
                dCar.fuel();
                eCar = (IElectric) dCar.getExtension(InterfaceID.ID_Electric);
                eCar.charge();
                
                factory = finder.findFactory(InterfaceID.ID_Hybridic);
                hCar = (IHybrid) factory.create();
                hCar.fuel();
                hCar.charge();
                
                // oder mit Rollenwechsel
                ICar car = null;
                factory = finder.findFactory(InterfaceID.ID_Hybrid); // supports only IDiesel und IElectric
                car = factory.create();
                ((IDiesel) car).fuel();
                ((IElectric) car).charge();
                
                car = car.getExtension(InterfaceID.ID_Diesel);  // Role ID_Diesel
                ((IDiesel) car).fuel();

                car = car.getExtension(InterfaceID.ID_Electric); // Role ID_Electric
                ((IElectric) car).charge();
                
                // car = car.getExtension(InterfaceID.ID_Hybrid);  // UnknownExtensionException: IHybrid not supported
                // ((IDiesel) car).fuel();
                // ((IElectric) car).charge();
                
                // car = car.getExtension(InterfaceID.ID_Hybridic); // UnknownExtensionException: IHybridic not supported
                // ((IHybrid) car).fuel();
                // ((IHybrid) car).charge();
                
                /* Result
                 
                    Filled up with Diesel
                    Charged with Electricity
                    Filled up with Diesel
                    Charged with Electricity
                    Filled up with Diesel
                    Charged with Electricity
                    Filled up with Diesel
                    Charged with Electricity
                    Filled up with Diesel
                    Charged with Electricity
                 
                 */
                
            }
            catch (UnknownExtensionException ex)
            {
                Console.WriteLine(ex);
                throw;
            }
        }
    }
   
    public interface ICar { ICar getExtension(int id); }  // IRoot
    public interface IDiesel : ICar { void fuel(); }
    public interface IElectric : ICar { void charge(); }
    public interface IHybrid : IElectric, IDiesel { }

    public class DieselCar: IDiesel
    {
        public ICar getExtension(int id)
        {
            switch (id) {
                case InterfaceID.ID_Root:
                case InterfaceID.ID_Diesel:
                    return this;
                default:
                    throw new UnknownExtensionException(id);
            }
        }
        public void fuel() { Console.WriteLine("Filled up with Diesel"); }
    }
    public class ElectricCar: IElectric
    {
        public ICar getExtension(int id)
        {
            switch (id) {
                case InterfaceID.ID_Root:
                case InterfaceID.ID_Electric:
                    return this;
                default:
                    throw new UnknownExtensionException(id);
            }
        }
        public void charge() { Console.WriteLine("Charged with Electricity"); }
    }
    public class HybridCar: IElectric, IDiesel
    {
        public ICar getExtension(int id)
        {
            switch (id) {
                case InterfaceID.ID_Root:
                case InterfaceID.ID_Diesel:
                case InterfaceID.ID_Electric:
                    return this;
                default:
                    throw new UnknownExtensionException(id);
            }
        }
        public void fuel() { Console.WriteLine("Filled up with Diesel"); }
        public void charge() { Console.WriteLine("Charged with Electricity"); }    
    }
    public class HybridicCar: IHybrid
    {
        public ICar getExtension(int id)
        {
            switch (id) {
                case InterfaceID.ID_Root:
                case InterfaceID.ID_Hybridic:
                    return this;
                default:
                    throw new UnknownExtensionException(id);
            }
        }
        public void fuel() { Console.WriteLine("Filled up with Diesel"); }
        public void charge() { Console.WriteLine("Charged with Electricity"); }    
    }
 
    
    public interface IFactory { ICar create(); }
    
    public class DieselFactory: IFactory { public ICar create() { return new DieselCar(); } }
    public class HybridFactory: IFactory { public ICar create() { return new HybridCar(); } }
    public class HybridicFactory: IFactory { public ICar create() { return new HybridicCar(); } }
    public class ElectricFactory: IFactory { public ICar create() { return new ElectricCar(); } }

    public class FactoryFinder   // Singleton
    {
        private Dictionary<int, IFactory> dictionary;
        private static FactoryFinder theInstance;

        public static FactoryFinder getInstance()
        {
            if(theInstance == null) theInstance = new FactoryFinder();
            return theInstance;
        }
        public FactoryFinder() { dictionary = new Dictionary<int, IFactory>(); }

        public void registerFactory(int id, IFactory factory) { dictionary[id] = factory; }
        public IFactory findFactory(int id) { return dictionary[id]; }
    }
    
    
    #region Helper
    
    public class UnknownExtensionException : Exception { public UnknownExtensionException(int id) { throw new Exception(string.Format("No extension with id {0} found", id)); } }
    public static class InterfaceID
    {
        public const int ID_Root = 0;
        public const int ID_Diesel = 1;
        public const int ID_Hybrid = 2;
        public const int ID_Hybridic = 3;
        public const int ID_Electric = 4;
    }

    public static class CarInstaller
    {
        public static void install()
        {
            var finder = FactoryFinder.getInstance();
            var dieselFactory = new DieselFactory();
            var hybridFactory = new HybridFactory();
            var hybridicFactory = new HybridicFactory();
            var electricFactory = new ElectricFactory();
            finder.registerFactory(InterfaceID.ID_Diesel, dieselFactory);
            finder.registerFactory(InterfaceID.ID_Electric, electricFactory);
            finder.registerFactory(InterfaceID.ID_Hybrid, hybridFactory);
            finder.registerFactory(InterfaceID.ID_Hybridic, hybridicFactory);
        }
    }
    
    #endregion
    
}