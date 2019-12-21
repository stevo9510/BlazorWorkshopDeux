using BandBookerData;
using BandBookerData.Models;
using System;
using System.Linq;

namespace BandBookerDataTest
{
    class Program
    {
        static void Main(string[] args)
        {
            // Un-comment to start over
            //DataManager.DeleteAll();

            AddNewInstrument("Guitar");
            AddNewInstrument("Keyboards");
        }

        private static void AddNewInstrument(string instrumentName)
        {
            var instrument = (from i in DataManager.Instruments
                          where i.Name == instrumentName
                          select i).FirstOrDefault();

            if (instrument == null)
            {
                instrument = new Instrument()
                {
                    Name = instrumentName
                };
                DataManager.AddInstrument(instrument);
            }
            Console.WriteLine("Instrument: {0} with InstrumentId {1}", instrument.Name, instrument.InstrumentId);
        }
    }
}