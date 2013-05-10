//using NUnit.Framework;

//namespace Coffee.Core.Tests
//{	
//	[TestFixture]
//	public class CoffeeHandlerTests
//	{
//		private const int CoffeMachineWeight = 2162;
//		private const int CoffeePotWeight = 390;
//		private const int CoffeeFilterWithCoffee = 100;
//		private const int OneCoffeeCupWeight = 125;

//		[Test]
//		public void HandleEvent_WeightIsZero_ZeroCups()
//		{
//			var coffeHandler = new CoffeeHandler();
//			coffeHandler.HandleEvent(new CoffeeDataChangedEvent { WeightInGrams = 0 });

//			Assert.That(coffeHandler.NumberOfCups, Is.EqualTo(0));
//		}

//		[Test]
//		public void HandleEvent_CoffeeMachineIsEmpty_ZeroCups()
//		{
//			var coffeHandler = new CoffeeHandler();
//			coffeHandler.HandleEvent(new CoffeeDataChangedEvent { Weight = CoffeMachineWeight });

//			Assert.That(coffeHandler.NumberOfCups, Is.EqualTo(0));
//		}

//		[Test]
//		public void HandleEvent_CoffeeMachineWithEmptyPot_ZeroCups()
//		{
//			var coffeHandler = new CoffeeHandler();
//			coffeHandler.HandleEvent(new CoffeeDataChangedEvent { Weight = CoffeMachineWeight + CoffeePotWeight });

//			Assert.That(coffeHandler.NumberOfCups, Is.EqualTo(0));
//		}

//		[Test]
//		public void HandleEvent_CoffeeMachineWithPotAndFilterAndOneCoffeeCup_OneCup()
//		{
//			var coffeHandler = new CoffeeHandler();
//			coffeHandler.HandleEvent(new CoffeeDataChangedEvent { Weight = CoffeMachineWeight + CoffeePotWeight + CoffeeFilterWithCoffee + OneCoffeeCupWeight });

//			Assert.That(coffeHandler.NumberOfCups, Is.EqualTo(1));
//		}

//		[Test]
//		public void HandleEvent_CoffeeMachineWithPotAndFilterAndTwoCoffeeCup_TwoCups()
//		{
//			var coffeHandler = new CoffeeHandler();
//			coffeHandler.HandleEvent(new CoffeeDataChangedEvent { Weight = CoffeMachineWeight + CoffeePotWeight + CoffeeFilterWithCoffee + OneCoffeeCupWeight + OneCoffeeCupWeight });

//			Assert.That(coffeHandler.NumberOfCups, Is.EqualTo(2));
//		}

//		[Test]
//		public void HandleEvent_CoffeeMachineWithPotAndFilterAndTwoCoffeeCupAndThePotIsRemoved_TwoCups()
//		{
//			var coffeHandler = new CoffeeHandler();
//			coffeHandler.HandleEvent(new CoffeeDataChangedEvent { Weight = CoffeMachineWeight + CoffeePotWeight + CoffeeFilterWithCoffee + OneCoffeeCupWeight + OneCoffeeCupWeight });
//			coffeHandler.HandleEvent(new CoffeeDataChangedEvent { Weight = CoffeMachineWeight + CoffeeFilterWithCoffee });

//			Assert.That(coffeHandler.NumberOfCups, Is.EqualTo(2));
//		}
//	}
//}