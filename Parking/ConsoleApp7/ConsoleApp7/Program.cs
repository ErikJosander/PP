using System;
using System.Collections.Generic;
using System.Linq;
using ClassLibrary1;
using ClassLibrary1.Data;
using Vehicles;
using ClassLibrary1.Models;
using System.Text;
using System.Threading.Tasks;
using System.Data.Entity;

namespace ConsoleApp7
{
    class Program
    {
        public static void Seed()
        {
            if (Repository.SeedDatabase() == true)
            {
                Console.WriteLine("New database created");
            }
            else
            {
                Console.WriteLine("Database allready exist");
            }
        }
        static void Main(string[] args)
        {
            Seed();

            bool programrunning = true;
            while (programrunning)
            {
                ConsoleHelper.MainWindow();
                int input = ConsoleHelper.ReadIntInput(9);
                switch (input)
                {
                    case 1:
                        CreateAndPlace();
                        break;
                    case 2:
                        RemoveVehicle();
                        break;
                    case 3:
                        ShowAllVehicles();
                        break;
                    case 4:
                        ShowHistory();
                        break;
                    case 5:
                        ReturnToMain();
                        break;
                    case 6:
                        MoveVehicle();
                        break;
                    case 7:
                        SortVehicles();
                        break;
                    case 8:
                        ShowList();
                        break;
                    case 9:
                        BetweenDatesList();
                        break;
                    default:
                        break;
                }
            }
        }
        public static void ReturnToMain()
        {

        }
        public static void CreateAndPlace()
        {
            var vehicle = CreateVehicle();
            ConsoleHelper.OutputLine("1 - Manual Place", false);
            ConsoleHelper.OutputLine("2 - Automatic Place");
            var input = ConsoleHelper.ReadIntInput(2);
            Console.WriteLine("Press any key to continue");
            Console.ReadLine();
            ConsoleHelper.ClearOutput();
            switch (input)
            {
                case 1:
                    PlaceVehicle(vehicle);
                    break;
                case 2:
                    PlaceAutomatic(vehicle, vehicle.IsCarMc);
                    break;
                default:
                    break;
            }
        }
        public static Vehicle CreateVehicle()
        {
            bool creatingvehicle = true;
            while (creatingvehicle)
            {
                ConsoleHelper.ClearOutput();
                bool carOrMc;
                Console.WriteLine("Enter regnumber: ");
                string regnumber = Console.ReadLine();
                regnumber = regnumber.ToLower();
                if (Repository.CheckIfRegExist(regnumber) == true)
                {
                    ConsoleHelper.ClearOutput();
                    Console.WriteLine("Reg allready exist");
                    Console.WriteLine("Press any key to continue");
                    Console.ReadLine();
                    regnumber = null;
                    continue;
                }
                if (ConsoleHelper.RegInputValidation(regnumber) == false)
                {
                    ConsoleHelper.ClearOutput();
                    Console.WriteLine("Error");
                    Console.WriteLine("Press any key to continue");
                    Console.ReadLine();
                    regnumber = null;
                    continue;
                }
                else
                {
                    ConsoleHelper.ClearOutput();
                    Console.WriteLine("Reg ok");
                    Console.WriteLine("Press any key to continue");
                    Console.ReadLine();
                }

                ConsoleHelper.ClearOutput();
                Console.WriteLine("Is it a car or mc?");
                Console.WriteLine("Car - 1");
                Console.WriteLine("Mc - 2");
                int input = ConsoleHelper.ReadIntInput(2);
                // Is Car
                if (input == 1)
                {
                    carOrMc = true;
                }
                // Is Mc
                else
                {
                    carOrMc = false;
                }
                Vehicle vehicle = new Vehicle(carOrMc, regnumber);
                ConsoleHelper.ClearOutput();
                Console.WriteLine("Vehicle Created");
                Console.WriteLine("Press any key to continue");
                Console.ReadLine();
                ConsoleHelper.ClearOutput();
                return vehicle;
            }
            Vehicle init = new Vehicle(true, "hej");
            return init;
        }
        public static void PlaceVehicle(Vehicle vehicle)
        {
            ConsoleHelper.OutputLine("Choose parkingspace to place vehicle", false);
            int input = ConsoleHelper.ReadIntInput(100);
            if (Repository.AddVehicleToSpace(vehicle, input))
            {
                ConsoleHelper.OutputLine("Vehicel parked");
                Console.WriteLine("Press any key to continue");
                Console.ReadLine();
                ConsoleHelper.ClearOutput();
            }
            else
            {
                ConsoleHelper.OutputLine("Vehicle not parked");
                Console.WriteLine("Press any key to continue");
                Console.ReadLine();
                ConsoleHelper.ClearOutput();
            }

        }
        public static void RemoveVehicle()
        {
            var regnumber = ConsoleHelper.ReadInput("Enter the regnumber of the vehicle you want to remove: ");
            if (Repository.RemoveVehicle(regnumber) == true)
            {
                Console.WriteLine("Vehicle removed");
            }
            else
            {
                Console.WriteLine("Vehicle not found");
            }
        }
        public static void ShowAllVehicles()
        {
            var listOfVehicles = Repository.GetListOfVehicles();
            foreach (var space in listOfVehicles)
            {
                Console.Write($"Space: {space.Id}");
                if (space.Empty == true)
                {
                    Console.Write(" is Empty");
                }
                //Car
                if (space.CarRegNumber != null)
                {
                    TimeSpan? interval = DateTime.Now - space.CarArrivedOn;
                    TimeSpan time = DateTime.Now - DateTime.Now;
                    if (interval.HasValue)
                    {
                        time = interval.Value;
                    }
                    var temptime = (int)time.TotalHours;
                    Console.Write($" Car: {space.CarRegNumber} - Time {temptime}/Hours");
                }
                // McSpotOne
                if (space.McOneRegNumber != null && space.McSlotTwoEmpty)
                {
                    TimeSpan? interval = DateTime.Now - space.McOneArrivedOn;
                    TimeSpan time = DateTime.Now - DateTime.Now;
                    if (interval.HasValue)
                    {
                        time = interval.Value;
                    }
                    var temptime = (int)time.TotalHours;
                    Console.Write($" Mc: {space.McOneRegNumber} - Time {temptime}/Hours");
                }
                // McSpotTwo
                if (space.McTwoRegNumber != null && space.McSlotOneEmpty)
                {
                    TimeSpan? interval = DateTime.Now - space.McTwoArrivedOn;
                    TimeSpan time = DateTime.Now - DateTime.Now;
                    if (interval.HasValue)
                    {
                        time = interval.Value;
                    }
                    var temptime = (int)time.TotalHours;
                    Console.Write($" Mc: {space.McTwoRegNumber} - Time {temptime}/Hours");
                }
                if (space.McSlotOneEmpty == false && space.McSlotTwoEmpty == false)
                {
                    // McOne
                    TimeSpan? interval = DateTime.Now - space.McOneArrivedOn;
                    TimeSpan time = DateTime.Now - DateTime.Now;
                    if (interval.HasValue)
                    {
                        time = interval.Value;
                    }
                    var temptime = (int)time.TotalHours;

                    //McTwo
                    TimeSpan? interval2 = DateTime.Now - space.McTwoArrivedOn;
                    TimeSpan time2 = DateTime.Now - DateTime.Now;
                    if (interval2.HasValue)
                    {
                        time2 = interval2.Value;
                    }
                    var temptime2 = (int)time.TotalHours;

                    Console.Write($" Mc: {space.McOneRegNumber} - Time {temptime}/Hours.  Mc: {space.McTwoRegNumber} - Time {temptime2}/Hours");
                }
                Console.WriteLine();
            }
        }
        public static void ShowHistory()
        {
            var listOfHistories = Repository.GetListOfHistory();
            foreach (var h in listOfHistories)
            {
                //TimeSpan time = DateTime.Now - DateTime.Now;
                //if (h.TimeStayed.HasValue)
                //{
                //    time = h.TimeStayed.Value;
                //}
                Console.WriteLine($"Reg: {h.VehicleRegNumber} - {h.TimeStayed}");
            }
        }
        public static void PlaceAutomatic(Vehicle vehicle, bool carOrMc)
        {
            if (Repository.PlaceAutomatic(vehicle, carOrMc) == true)
            {
                Console.WriteLine("Parked");
            }
            else
            {
                Console.WriteLine("Parking is full");
            }

        }
        public static void MoveVehicle()
        {
            bool moveVehicleRunning = true;
            while (moveVehicleRunning)
            {
                var regnumber = ConsoleHelper.ReadInput("Enter regnumber:", false);

                if (Repository.CheckIfRegExist(regnumber))
                {
                    var idToFind = Repository.ReturnIdOfRegnumber(regnumber);
                    ConsoleHelper.OutputLine("Enter space to move to", false);
                    var spaceToMoveto = ConsoleHelper.ReadIntInput(100);
                    if (Repository.MoveVehicle(regnumber, idToFind, spaceToMoveto) == true)
                    {
                        Console.WriteLine("vehicle moved");
                        moveVehicleRunning = false;
                    }
                    else
                    {
                        Console.WriteLine("vehicle nor moved");
                        moveVehicleRunning = false;
                    }
                }
                else
                {
                    ConsoleHelper.OutputLine("Not found");
                    moveVehicleRunning = false;
                }
            }
        }
        public static void SortVehicles()
        {
            Repository.SortSpaces();
            Console.WriteLine("Sorted");
            Console.WriteLine("Press any key to continue");
            Console.ReadLine();
            ConsoleHelper.ClearOutput();
        }
        public static void ShowList()
        {
            var histories = Repository.GetMostPayingCustomers();
            foreach (var history in histories)
            {
                Console.WriteLine($"Reg:{history.Regnumber} - Total:{history.TickerPrice}");
            }
        }
        public static void BetweenDatesList()
        {
            bool betweenDatesRunning = true;
            while (betweenDatesRunning)
            {
                DateTime beginningDate;
                DateTime endingDate;
                ConsoleHelper.OutputLine("Enter beginning year: ", false);
                var beginningDateYear = ConsoleHelper.ReadIntInput(3000);
                ConsoleHelper.OutputLine("Enter beginning month: ", false);
                var beginningDateMonth = ConsoleHelper.ReadIntInput(12);
                ConsoleHelper.OutputLine("Enter beginning day: ", false);
                var beginningDateDay = ConsoleHelper.ReadIntInput(31);

                ConsoleHelper.OutputLine("Enter ending year: ", false);
                var endingDateYear = ConsoleHelper.ReadIntInput(3000);
                ConsoleHelper.OutputLine("Enter ending month: ", false);
                var endingDateMonth = ConsoleHelper.ReadIntInput(12);
                ConsoleHelper.OutputLine("Enter ending day: ", false);
                var endingDateDay = ConsoleHelper.ReadIntInput(31);


                try
                {
                    beginningDate = new DateTime(beginningDateYear, beginningDateMonth, beginningDateDay, 0, 0, 0);
                    endingDate = new DateTime(endingDateYear, endingDateMonth, endingDateDay, 0, 0, 0);

                }
                catch (ArgumentOutOfRangeException)
                {
                    Console.WriteLine("No such date exists");
                    continue;
                    throw;
                }
                if(beginningDate > DateTime.Now || endingDate > DateTime.Now)
                {
                    ConsoleHelper.ClearOutput();
                    Console.WriteLine("Dates in future");
                    Console.WriteLine("Press any key to continue");
                    Console.ReadLine();
                    ConsoleHelper.ClearOutput();
                    continue;
                }
                if (beginningDate > endingDate)
                {
                    ConsoleHelper.ClearOutput();
                    Console.WriteLine("Ending date before begining date");
                    Console.WriteLine("Press any key to continue");
                    Console.ReadLine();
                    ConsoleHelper.ClearOutput();
                    continue;
                }


                var intervalList = Repository.GetIntervalAndDayPrice(beginningDate, endingDate);
                Console.WriteLine(intervalList.Count());
                Console.WriteLine("Regnumbers of vehicles:");
                int daysDiff = ((TimeSpan)(endingDate - beginningDate)).Days;
                Console.WriteLine($"Num of day: {daysDiff.ToString()}");
                int price = 0;
                foreach (var x in intervalList)
                {
                    price = price + x.TicketPrice;
                }
                int pricePerDay = price / daysDiff;
                Console.WriteLine($"Price per day: {pricePerDay.ToString()}");

                foreach (var history in intervalList)
                {
                    Console.WriteLine($"{history.VehicleRegNumber}");
                }
                betweenDatesRunning = false;
            }
        }
    }
}
