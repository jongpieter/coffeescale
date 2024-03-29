﻿namespace Coffee.Core
{
	public class CoffeeHandler
	{
		public CoffeeMachineStatus Status { get; set; }
		private const decimal NoCoffeeWeight = 2526 + 364;
		private const decimal OneCoffeeCupWeight = 108.6M;

		public void HandleEvent(CoffeeDataChangedEvent coffeeDataChangedEvent)
		{
			if (coffeeDataChangedEvent.Weight < NoCoffeeWeight)
			{				
				if(Status == CoffeeMachineStatus.Unknown)
					NumberOfCups = 0;
				Status = CoffeeMachineStatus.PotIsMissing;
			}
			else
			{
				Status = CoffeeMachineStatus.PotInMachine;
				NumberOfCups = (coffeeDataChangedEvent.Weight - NoCoffeeWeight) / OneCoffeeCupWeight;
			}
		}

		public decimal NumberOfCups { get; set; }
	}
	
	public enum CoffeeMachineStatus
	{
		Unknown,
		PotIsMissing,
		PotInMachine
	}
}