using System;
using System.Globalization;

namespace InterviewChallenge
{
	//The AA field
	class Customer
	{
		public string Invoice_No;
		public string Account_No;
		public string Customer_Name;
		public string Cycle_Cd;
		public string Bill_Dt;
		public string Due_Dt;
		public string Bill_Amount;
		public string Balance_Due;
		public string Bill_Run_Dt;
		public string Bill_Run_Seq;
		public string Bill_Run_Tm;
		public string Bill_Tp;
		public string Mailing_Address_1;
		public string Mailing_Address_2;
		public string City;
		public string State;
		public string Zip;
		public string Account_Class;

		static string invoiceFormat = "8E2FEA69-5D77-4D0F-898E-DFA25677D19E";

		public void DisplayAAInfo()
		{
			//Format and calculate the needed dates
			string crrntDate = DateTime.Now.ToString("MM/dd/yyyy");
			string firstNotify = DateTime.Now.AddDays(5).ToString("MM-dd-yyyy");
			string secondNotify = DateTime.Parse(Due_Dt).AddDays(-3).ToString("MM-dd-yyyy");
			Bill_Dt = DateTime.Parse(Bill_Dt).ToString("MM/dd/yyyy", CultureInfo.InvariantCulture);
			Due_Dt = DateTime.Parse(Due_Dt).ToString("MM/dd/yyyy", CultureInfo.InvariantCulture);

			//Displaying the AA && the HH Line
			DisplayAandHHline(crrntDate, firstNotify, secondNotify);
		}

		private void DisplayAandHHline(string crrntDate, string firstNotify, string secondNotify)
		{
			Console.Write("AA~CT");
			if (Account_No.Length >= 1)
				Console.Write("|BB~" + Account_No);
			if (Customer_Name.Length >= 1)
				Console.Write("|VV~" + Customer_Name);
			if (Mailing_Address_1.Length >= 1)
				Console.Write("|CC~" + Mailing_Address_1);
			if (Mailing_Address_2.Length >= 1)
				Console.Write("|DD~" + Mailing_Address_2);
			if (City.Length >= 1)
				Console.Write("|EE~" + City);
			if (State.Length >= 1)
				Console.Write("|FF~" + State);
			if (Zip.Length >= 1)
				Console.WriteLine("|GG~" + Zip);
			Console.Write("HH~IH");
			Console.Write("|II~R");
			if (invoiceFormat.Length >= 1)
				Console.Write("|JJ~" + invoiceFormat);
			if (Invoice_No.Length >= 1)
				Console.Write("|KK~" + Invoice_No);
			if (Bill_Dt.Length >= 1)
				Console.Write("|LL~" + Bill_Dt);
			if (Due_Dt.Length >= 1)
				Console.Write("|MM~" + Due_Dt);
			if (Bill_Amount.Length >= 1)
				Console.Write("|NN~" + Bill_Amount);
			if (firstNotify.Length >= 1)
				Console.Write("|OO~" + firstNotify);
			if (secondNotify.Length >= 1)
				Console.Write("|PP~" + secondNotify);
			if (Balance_Due.Length >= 1)
				Console.Write("|QQ~" + Balance_Due);
			Console.Write("|RR~" + crrntDate);
			//if there is only one mailing address on either 1 or 2
			if (Mailing_Address_1.Length >= 1)
				Console.WriteLine("|SS~" + Mailing_Address_1);
			else if (Mailing_Address_2.Length >= 1)
				Console.WriteLine("|SS~" + Mailing_Address_2);
		}
	}
}
