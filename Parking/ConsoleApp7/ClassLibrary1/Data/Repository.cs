using System;
using System.Diagnostics;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using ClassLibrary1.Models;
using Vehicles;

namespace ClassLibrary1.Data
{
    /// <summary>
    /// Repository class that provides various database queries
    /// and CRUD operations.
    /// </summary>
    public static class Repository
    {
        /// <summary>
        /// Private method that returns a database context.
        /// Only to be used within repository
        /// </summary>
        /// <returns>An instance of the Context class.</returns>
        static Context GetContext()
        {
            var context = new Context();
            context.Database.Log = (message) => Debug.WriteLine(message);
            return context;
        }

        /// <summary>
        /// Seeding Databasewith 100 Spaces if no exist return false
        /// else returns true
        /// </summary>
        /// <returns>An IList collection of ComicBook entity instances.</returns>
        public static bool SeedDatabase()
        {
            using (Context context = GetContext())
            {
                int exists = context.Spaces.Count();
                if (exists == 0)
                {
                    for (int i = 0; i < 100; i++)
                    {
                        context.Spaces.Add(new Space());
                    }
                    context.SaveChanges();
                    return true;
                }
                else
                {
                    return false;
                }
            }
        }

        /// <summary>
        /// Adds car to space, return false if parkingspace is not available 
        /// </summary>
        /// <param name="car"></param>
        /// <param name="id"></param>
        /// <returns></returns>
        public static bool AddVehicleToSpace(Vehicle vehicle, int id)
        {
            using (Context context = GetContext())
            {
                // car parking
                if (vehicle.IsCarMc == true)
                {
                    Space space = context.Spaces.Find(id);
                    if (space.Empty == true)
                    {
                        space.CarRegNumber = vehicle.RegNumber;
                        space.Empty = false;
                        space.CarArrivedOn = DateTime.Now;
                        context.SaveChanges();
                        return true;
                    }
                    else
                    {
                        return false;
                    }
                }

                // mc parking
                else
                {
                    Space space = context.Spaces.Find(id);
                    if (space.Empty == true)
                    {
                        space.McOneRegNumber = vehicle.RegNumber;
                        space.McOneArrivedOn = DateTime.Now;
                        space.Empty = false;
                        space.McSlotOneEmpty = false;
                        context.SaveChanges();
                        return true;
                    }
                    if (space.McSlotOneEmpty == true)
                    {
                        space.McOneRegNumber = vehicle.RegNumber;
                        space.McOneArrivedOn = DateTime.Now;
                        space.Empty = false;
                        space.McSlotOneEmpty = false;
                        context.SaveChanges();
                        return true;
                    }
                    if (space.McSlotTwoEmpty == true)
                    {
                        space.McTwoRegNumber = vehicle.RegNumber;
                        space.McTwoArrivedOn = DateTime.Now;
                        space.Empty = false;
                        space.McSlotTwoEmpty = false;
                        context.SaveChanges();
                        return true;
                    }
                    else
                    {
                        return false;
                    }
                }
            }
        }

        /// <summary>
        /// Checks if a regnumber exists in database.
        /// </summary>
        /// <param name="regnumber"></param>
        /// <returns></returns>
        public static bool CheckIfRegExist(string regnumber)
        {
            List<string> regnumbers = new List<string>();
            using (Context context = GetContext())
            {
                var spaces = context.Spaces;

                foreach (var space in spaces)
                {
                    regnumbers.Add(space.CarRegNumber);
                    regnumbers.Add(space.McOneRegNumber);
                    regnumbers.Add(space.McTwoRegNumber);
                }
                if (regnumbers.Contains(regnumber))
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
        }

        /// <summary>
        /// Removes a vehicle if reg exists
        /// </summary>
        /// <param name="regnumber"></param>
        /// <returns></returns>
        public static bool RemoveVehicle(string regnumber)
        {
            if (CheckIfRegExist(regnumber) == true)
            {
                // 1 for car, 2 for mcslotone, 3 for mcslottwo
                int spot = CarOrMcOnSpot(regnumber);
                using (Context context = GetContext())
                {
                    Space spaceToUpdate = ReturnSpaceOfRegnumber(regnumber);
                    // Car
                    if (spot == 1)
                    {
                        DateTime arrivedOn;
                        arrivedOn = spaceToUpdate.CarArrivedOn.GetValueOrDefault(DateTime.Now);
                        int ticketprice = GetTicketPrice(arrivedOn, true);
                        AddToHistory(spaceToUpdate.CarRegNumber, "Car", spaceToUpdate.CarArrivedOn, true, ticketprice);
                        ClearSpace(spaceToUpdate);
                        context.Entry(spaceToUpdate).State = EntityState.Modified;
                        context.SaveChanges();
                        return true;
                    }
                    // Mc spot 1
                    if (spot == 2 && spaceToUpdate.McSlotTwoEmpty == true)
                    {
                        DateTime arrivedOn;
                        arrivedOn = spaceToUpdate.McOneArrivedOn.GetValueOrDefault(DateTime.Now);
                        int ticketprice = GetTicketPrice(arrivedOn, true);
                        AddToHistory(spaceToUpdate.McOneRegNumber, "Mc", spaceToUpdate.McOneArrivedOn, true, ticketprice);
                        ClearSpace(spaceToUpdate);
                        context.Entry(spaceToUpdate).State = EntityState.Modified;
                        context.SaveChanges();
                        return true;
                    }
                    // mc spot 2
                    if (spot == 3 && spaceToUpdate.McSlotOneEmpty == true)
                    {
                        DateTime arrivedOn;
                        arrivedOn = spaceToUpdate.McTwoArrivedOn.GetValueOrDefault(DateTime.Now);
                        int ticketprice = GetTicketPrice(arrivedOn, true);
                        AddToHistory(spaceToUpdate.McTwoRegNumber, "Mc", spaceToUpdate.McTwoArrivedOn, true, ticketprice);
                        ClearSpace(spaceToUpdate);
                        context.Entry(spaceToUpdate).State = EntityState.Modified;
                        context.SaveChanges();
                        return true;
                    }
                    //Mc spot 1
                    if (spot == 2 && spaceToUpdate.McSlotTwoEmpty == false)
                    {
                        DateTime arrivedOn;
                        arrivedOn = spaceToUpdate.McOneArrivedOn.GetValueOrDefault(DateTime.Now);
                        int ticketprice = GetTicketPrice(arrivedOn, true);
                        AddToHistory(spaceToUpdate.McOneRegNumber, "Mc", spaceToUpdate.McOneArrivedOn, true, ticketprice);
                        spaceToUpdate.McOneRegNumber = null;
                        spaceToUpdate.McOneArrivedOn = null;
                        spaceToUpdate.McSlotOneEmpty = true;
                        context.Entry(spaceToUpdate).State = EntityState.Modified;
                        context.SaveChanges();
                        return true;
                    }
                    // mc spot two
                    if (spot == 3 && spaceToUpdate.McSlotOneEmpty == false)
                    {
                        DateTime arrivedOn;
                        arrivedOn = spaceToUpdate.McTwoArrivedOn.GetValueOrDefault(DateTime.Now);
                        int ticketprice = GetTicketPrice(arrivedOn, true);
                        AddToHistory(spaceToUpdate.McTwoRegNumber, "Mc", spaceToUpdate.McTwoArrivedOn, true, ticketprice);
                        spaceToUpdate.McTwoRegNumber = null;
                        spaceToUpdate.McTwoArrivedOn = null;
                        spaceToUpdate.McSlotTwoEmpty = true;
                        context.Entry(spaceToUpdate).State = EntityState.Modified;
                        context.SaveChanges();
                        return true;
                    }
                    else
                    {
                        return false;
                    }
                }
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// Adds removed vehicledata to the history-table in the parkinglot Database.
        /// </summary>
        /// <param name="regnumber"></param>
        /// <param name="carOrMc"></param>
        /// <param name="arrivedOn"></param>
        /// <param name="leavedOn"></param>
        /// <param name="payedTicket"></param>
        /// <param name="ticketPrice"></param>
        public static void AddToHistory(string regnumber, string carOrMc, DateTime? arrivedOn, bool payedTicket, int ticketPrice)
        {
            using (Context context = GetContext())
            {
                TimeSpan? interval = DateTime.Now - arrivedOn;
                TimeSpan time = DateTime.Now - DateTime.Now;
                if (interval.HasValue)
                {
                    time = interval.Value;
                }
                var temptime = (int)time.TotalHours;

                History removedVehicle = new History()
                {
                    VehicleRegNumber = regnumber,
                    CarOrMC = carOrMc,
                    ArrivedOn = arrivedOn,
                    LeavedOn = DateTime.Now,
                    TimeStayed = temptime,
                    PayedTicket = payedTicket,
                    TicketPrice = ticketPrice
                };
                context.Histories.Add(removedVehicle);
                context.SaveChanges();
            }
        }

        /// <summary>
        /// Get ticketprice as an int, (Datetime? need to be convertet before use)
        /// </summary>
        /// <param name="arrivedOn"></param>
        /// <param name="leavedOn"></param>
        /// <param name="carOrMc"></param>
        /// <returns></returns>
        public static int GetTicketPrice(DateTime arrivedOn, bool carOrMc)
        {
            int toPay = 0;
            var tempTime = DateTime.Now - arrivedOn;
            double time = tempTime.TotalMinutes;
            if (time < 5)
            {
                return toPay;
            }
            double timeToHours = tempTime.TotalHours;
            if (carOrMc == true)
            {
                toPay = 20;
                if (timeToHours < 2)
                {
                    return toPay * 2;
                }
                if (timeToHours > 3)
                {
                    return toPay * (int)timeToHours;
                }
                else return toPay;
            }
            if (carOrMc == false)
            {
                toPay = 10;
                if (timeToHours < 2)
                {
                    return toPay * 2;
                }
                if (timeToHours > 3)
                {
                    return toPay * (int)timeToHours;
                }
                else return toPay;
            }
            else return toPay;
        }

        /// <summary>
        /// Reset a Space entity to init
        /// </summary>
        /// <param name="space"></param>
        public static void ClearSpace(Space space)
        {
            space.Empty = true;
            space.CarArrivedOn = null;
            space.CarRegNumber = null;
            space.McOneArrivedOn = null;
            space.McOneRegNumber = null;
            space.McSlotOneEmpty = true;
            space.McTwoArrivedOn = null;
            space.McTwoRegNumber = null;
            space.McSlotTwoEmpty = true;
        }

        /// <summary>
        /// Reavels on what spot the vehicle is on
        /// 1 = car, 2 = mc spot one, 3 = mc spot two, 4 = false
        /// </summary>
        /// <param name="space"></param>
        /// <returns></returns>
        public static int CarOrMcOnSpot(string regnumber)
        {
            var space = ReturnSpaceOfRegnumber(regnumber);
            using (Context context = GetContext())
            {
                if (space.CarRegNumber == regnumber)
                {
                    return 1;
                }
                if (space.McOneRegNumber == regnumber)
                {
                    return 2;
                }
                if (space.McTwoRegNumber == regnumber)
                {
                    return 3;
                }
                else return 4;
            }
        }

        /// <summary>
        /// Retunr the id of the space that contaiens regnumber, return 0 if not found
        /// </summary>
        /// <param name="regnumber"></param>
        /// <returns></returns>
        public static int ReturnIdOfRegnumber(string regnumber)
        {
            using (Context context = GetContext())
            {
                var spaces = context.Spaces;
                foreach (var space in spaces)
                {
                    if (space.CarRegNumber == regnumber)
                    {
                        return space.Id;
                    }
                    if (space.McOneRegNumber == regnumber)
                    {
                        return space.Id;
                    }
                    if (space.McTwoRegNumber == regnumber)
                    {
                        return space.Id;
                    }
                }
                return 0;
            }
        }

        /// <summary>
        /// Works toghter with ReturnIdOfRegnumber() to return certain space by regnumber
        /// </summary>
        /// <param name="regnumber"></param>
        /// <returns></returns>
        public static Space ReturnSpaceOfRegnumber(string regnumber)
        {
            int id = ReturnIdOfRegnumber(regnumber);
            using (Context context = GetContext())
            {
                var space = context.Spaces.Find(id);
                return space;
            }
        }

        /// <summary>
        /// Gets a list of the Spaces table
        /// </summary>
        /// <returns></returns>
        public static List<Space> GetListOfVehicles()
        {
            List<Space> listOfVehicles = new List<Space>();
            using (Context context = GetContext())
            {
                var spaces = context.Spaces;
                foreach (var space in spaces)
                {
                    listOfVehicles.Add(space);
                }
                return listOfVehicles;
            }
        }

        /// <summary>
        /// Gets a list of the Histories table
        /// </summary>
        /// <returns></returns>
        public static List<History> GetListOfHistory()
        {
            List<History> listOfHisitories = new List<History>();
            using (Context context = GetContext())
            {
                var histories = context.Histories;
                foreach (var enity in histories)
                {
                    listOfHisitories.Add(enity);
                }
                return listOfHisitories;
            }
        }

        /// <summary>
        /// Places a vehicle automaticly, return false if parkinglot is full
        /// </summary>
        /// <param name="vehicle"></param>
        /// <param name="carOrMc"></param>
        /// <returns></returns>
        public static bool PlaceAutomatic(Vehicle vehicle, bool carOrMc)
        {
            using (Context context = GetContext())
            {
                var spaces = context.Spaces;
                if (carOrMc == true)
                {
                    foreach (var space in spaces)
                    {
                        if (space.Empty == true)
                        {
                            AddVehicleToSpace(vehicle, space.Id);
                            context.SaveChanges();
                            return true;
                        }
                    }
                }
                if (carOrMc == false)
                {
                    foreach (var space in spaces)
                    {
                        if (space.Empty == true)
                        {
                            AddVehicleToSpace(vehicle, space.Id);
                            context.SaveChanges();
                            return true;
                        }
                        if (space.McSlotOneEmpty == true && space.CarRegNumber == null)
                        {
                            AddVehicleToSpace(vehicle, space.Id);
                            context.SaveChanges();
                            return true;
                        }
                        if (space.McSlotTwoEmpty == true && space.CarRegNumber == null)
                        {
                            AddVehicleToSpace(vehicle, space.Id);
                            context.SaveChanges();
                            return true;
                        }
                    }
                }
            }
            return false;
        }

        /// <summary>
        /// move vehicle to new spot
        /// </summary>
        /// <param name="regnumber"></param>
        /// <param name="idToFind"></param>
        /// <param name="idToMoveTo"></param>
        /// <returns></returns>
        public static bool MoveVehicle(string regnumber, int idToFind, int idToMoveTo)
        {
            using (Context context = GetContext())
            {
                var findSpace = ReturnIdOfRegnumber(regnumber);
                var spaceToFind = GetSpace(idToFind);
                if (findSpace == 0)
                {
                    return false;
                }
                if (CarOrMcOnSpot(regnumber) == 1)
                {
                    DateTime? tempdate = spaceToFind.CarArrivedOn;
                    string tempreg = spaceToFind.CarRegNumber;
                    Vehicle vehicle = new Vehicle(true, tempreg);

                    if (AddVehicleToSpace(vehicle, idToMoveTo) == true)
                    {
                        spaceToFind.CarArrivedOn = null;
                        spaceToFind.CarRegNumber = null;
                        spaceToFind.Empty = true;
                        context.Entry(spaceToFind).State = EntityState.Modified;


                        var spaceToMoveTo = GetSpace(idToMoveTo);
                        spaceToMoveTo.CarArrivedOn = tempdate;
                        context.Entry(spaceToMoveTo).State = EntityState.Modified;
                        context.SaveChanges();
                        return true;
                    }
                    else
                    {
                        return false;
                    }

                }
                if (CarOrMcOnSpot(regnumber) == 2)
                {
                    DateTime? tempdate = spaceToFind.McOneArrivedOn;
                    string tempreg = spaceToFind.McOneRegNumber;
                    Vehicle vehicle = new Vehicle(false, tempreg);

                    if (AddVehicleToSpace(vehicle, idToMoveTo) == true)
                    {
                        spaceToFind.McOneArrivedOn = null;
                        spaceToFind.McOneRegNumber = null;
                        spaceToFind.McSlotOneEmpty = true;
                        if (spaceToFind.McSlotTwoEmpty == true)
                        {
                            spaceToFind.Empty = true;
                        }
                        context.Entry(spaceToFind).State = EntityState.Modified;


                        var spaceToMoveTo = GetSpace(idToMoveTo);
                        spaceToMoveTo.McOneArrivedOn = tempdate;
                        context.Entry(spaceToMoveTo).State = EntityState.Modified;
                        context.SaveChanges();
                        return true;
                    }
                    else
                    {
                        return false;
                    }
                }
                if (CarOrMcOnSpot(regnumber) == 3)
                {
                    DateTime? tempdate = spaceToFind.McTwoArrivedOn;
                    string tempreg = spaceToFind.McTwoRegNumber;
                    Vehicle vehicle = new Vehicle(false, tempreg);

                    if (AddVehicleToSpace(vehicle, idToMoveTo) == true)
                    {
                        spaceToFind.McTwoArrivedOn = null;
                        spaceToFind.McTwoRegNumber = null;
                        spaceToFind.McSlotTwoEmpty = true;
                        if (spaceToFind.McSlotOneEmpty == true)
                        {
                            spaceToFind.Empty = true;
                        }
                        context.Entry(spaceToFind).State = EntityState.Modified;


                        var spaceToMoveTo = GetSpace(idToMoveTo);
                        spaceToMoveTo.McTwoArrivedOn = tempdate;
                        context.Entry(spaceToMoveTo).State = EntityState.Modified;
                        context.SaveChanges();
                        return true;
                    }
                    else
                    {
                        return false;
                    }
                }
                else
                {
                    return false;
                }
            }
        }

        /// <summary>
        /// Get space entity, need to input 1-100
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public static Space GetSpace(int id)
        {
            using (Context context = GetContext())
            {
                return context.Spaces.Find(id);
            }
        }

        /// <summary>
        /// Sort all vehicles in alphabetic order
        /// </summary>
        public static void SortSpaces()
        {
            var tempVehicles = new List<TempVehicle>();
            var spaces = new List<Space>();
            using (Context context = GetContext())
            {
                spaces = context.Spaces.OrderBy(v => v.CarRegNumber)
                    .ThenBy(mc => mc.McOneRegNumber)
                    .ThenBy(mc => mc.McTwoRegNumber)
                    .ThenBy(a => a.Empty == true)
                    .ToList();
                foreach (var space in spaces)
                {
                    if (space.CarArrivedOn != null)
                    {
                        TempVehicle tempVehicle = new TempVehicle()
                        {
                            CarOrMC = true,
                            RegNumber = space.CarRegNumber,
                            ArrivedOn = space.CarArrivedOn
                        };
                        tempVehicles.Add(tempVehicle);
                    }

                }
                foreach (var space in spaces)
                {
                    if (space.McOneRegNumber != null)
                    {
                        TempVehicle tempVehicle = new TempVehicle()
                        {
                            CarOrMC = false,
                            RegNumber = space.McOneRegNumber,
                            ArrivedOn = space.McOneArrivedOn
                        };
                        tempVehicles.Add(tempVehicle);
                    }
                    if (space.McTwoRegNumber != null)
                    {
                        TempVehicle tempVehicle = new TempVehicle()
                        {
                            CarOrMC = false,
                            RegNumber = space.McTwoRegNumber,
                            ArrivedOn = space.McTwoArrivedOn
                        };
                        tempVehicles.Add(tempVehicle);
                    }
                }
                foreach (var space in spaces)
                {
                    ClearSpace(space);
                    context.Entry(space).State = EntityState.Modified;
                    context.SaveChanges();
                }
                foreach (var v in tempVehicles)
                {
                    if (v.CarOrMC == true)
                    {
                        Vehicle vehicle = new Vehicle(true, v.RegNumber);
                        PlaceAutomatic(vehicle, true);
                        var spaceToUpdate = ReturnSpaceOfRegnumber(v.RegNumber);
                        UpdateArrivedOn(v.RegNumber, spaceToUpdate, v.ArrivedOn);
                        context.SaveChanges();
                    }
                }
                foreach (var mc in tempVehicles)
                {
                    if (mc.CarOrMC == false)
                    {
                        Vehicle vehicle = new Vehicle(false, mc.RegNumber);
                        PlaceAutomatic(vehicle, false);
                        var spaceToUpdate = ReturnSpaceOfRegnumber(mc.RegNumber);
                        UpdateArrivedOn(mc.RegNumber, spaceToUpdate, mc.ArrivedOn);
                        context.SaveChanges();
                    }
                }
                context.SaveChanges();
                tempVehicles.Clear();
            }
        }

        /// <summary>
        /// update arrivedOn in a Space, onlt to be used with SortSpaces Methode
        /// </summary>
        /// <param name="regnum"></param>
        public static void UpdateArrivedOn(string regnum, Space space, DateTime? arrivedOn)
        {
            using (Context context = GetContext())
            {
                if (CarOrMcOnSpot(regnum) == 1)
                {
                    space.CarArrivedOn = arrivedOn;
                    context.Entry(space).State = EntityState.Modified;
                }
                if (CarOrMcOnSpot(regnum) == 2)
                {
                    space.McOneArrivedOn = arrivedOn;
                    context.Entry(space).State = EntityState.Modified;
                }
                if (CarOrMcOnSpot(regnum) == 3)
                {
                    space.McTwoArrivedOn = arrivedOn;
                    context.Entry(space).State = EntityState.Modified;
                }
                context.SaveChanges();
            }
        }

        /// <summary>
        /// Return a list of temp that contains a group of regnumber and sum of ticketprice
        /// </summary>
        /// <returns></returns>
        public static List<Temp> GetMostPayingCustomers()
        {
            List<Temp> temps = new List<Temp>();
            using (Context context = GetContext())
            {

                var mostPayingList = context.Histories
                    .GroupBy(r => r.VehicleRegNumber)
                    .Select(g => new { regnumber = g.Key, total = g.Sum(i => i.TicketPrice) })
                    .Take(10);

                foreach (var x in mostPayingList)
                {
                    Temp temp = new Temp()
                    {
                        Regnumber = x.regnumber,
                        TickerPrice = x.total
                    };
                    temps.Add(temp);
                }
                return temps;
            }
        }
        public static List<History> GetIntervalAndDayPrice(DateTime beginning, DateTime ending)
        {
            List<History> temps = new List<History>();
            using (Context context = GetContext())
            {
                var intervalList = context.Histories.Where(t => t.ArrivedOn >= beginning && t.ArrivedOn <= ending);
                foreach(var x in intervalList)
                {
                    History history = new History()
                    {
                        Id = x.Id,
                        VehicleRegNumber = x.VehicleRegNumber,
                        CarOrMC = x.CarOrMC,
                        ArrivedOn = x.ArrivedOn,
                        LeavedOn = x.LeavedOn,
                        PayedTicket = x.PayedTicket,
                        TimeStayed = x.TimeStayed,
                        TicketPrice = x.TicketPrice
                    };
                    temps.Add(history);
                }
                return temps;
            }
        }
    }
}
