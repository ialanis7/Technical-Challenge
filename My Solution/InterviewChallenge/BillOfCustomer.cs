using System;
using System.Data.OleDb;


//This will be the DB Stuff
namespace InterviewChallenge
{
	class BillOfCustomer
	{
		

		public string name;
		public string acctNum;
		public string addr;
		public string city;
		public string state;
		public string zip;
		public string dateAdded;
		public string billDt;
		public string billNum;
		public string billAmnt;
		public string formatGUID;
		public string accntBalance;
		public string dueDate;
		public string serviceAddress;
		public string firstEmailDate;
		public string secondEmailDate;


		public void addToCustomerTable()
		{
			//DB Stuff
			string conString = @"Provider=Microsoft.ACE.OLEDB.12.0;Data Source=C:\SPECIFY FILEPATH;Persist Security Info = False; ";
			OleDbConnection con = new OleDbConnection(conString);
			OleDbCommand cmd;

			//SQL Statement
			string sql = "INSERT INTO Customer(CustomerName,AccountNumber,CustomerAddress,CustomerCity,CustomerState,CustomerZip,DateAdded)" +
				"VALUES(@CustomerName,@AccountNumber,@CustomerAddress,@CustomerCity,@CustomerState,@CustomerZip,@DateAdded)";
			cmd = new OleDbCommand(sql, con);

			//Adding params
			cmd.Parameters.Add("@CustomerName", name);
			cmd.Parameters.Add("@AccountNumber", acctNum);
			cmd.Parameters.Add("@CustomerAddress", addr);
			cmd.Parameters.Add("@CustomerCity", city);
			cmd.Parameters.Add("@CustomerState", state);
			cmd.Parameters.Add("@CustomerZip", zip);
			cmd.Parameters.Add("@DateAdded", dateAdded);


			//Open con and execute insert
			InsertToCustomerDB(con, cmd);
		}

		private static void InsertToCustomerDB(OleDbConnection con, OleDbCommand cmd)
		{
			try
			{
				con.Open();
				//if we affected any rows
				SuccessMessage(cmd);
				//close connection
				con.Close();
			}
			catch (Exception ex)
			{
				Console.WriteLine(ex.Message);
				//close connection
				con.Close();
			}
		}

		private static void SuccessMessage(OleDbCommand cmd)
		{
			if (cmd.ExecuteNonQuery() > 0)
			{
				Console.WriteLine("Successfully Inserted to Customer table!");
			}
		}

		public void addToBillsTable(int j)
		{
			//DB Stuff
			string conString = @"Provider=Microsoft.ACE.OLEDB.12.0;Data Source=C:\SPECIFY FILEPATH;Persist Security Info = False; ";
			OleDbConnection con = new OleDbConnection(conString);
			OleDbCommand cmd;

			//SQL Statement
			string sql = "INSERT INTO Bills(BillDate,BillNumber,BillAmount,FormatGUID,AccountBalance,DueDate,ServiceAddress,FirstEmailDate,SecondEmailDate,DateAdded,CustomerID)" +
				"VALUES(@BillDate,@BillNumber,@BillAmount,@FormatGUID,@AccountBalance,@DueDate,@ServiceAddress,@FirstEmailDate,@SecondEmailDate,@DateAdded,@CustomerID)";
			cmd = new OleDbCommand(sql, con);

			//Adding params
			cmd.Parameters.Add("@BillDate", billDt);
			cmd.Parameters.Add("@BillNumber", billNum);
			cmd.Parameters.Add("@BillAmount", billAmnt);
			cmd.Parameters.Add("@FormatGUID", formatGUID);
			cmd.Parameters.Add("@AccountBalance", accntBalance);
			cmd.Parameters.Add("@DueDate", dueDate);
			cmd.Parameters.Add("@ServiceAddress", serviceAddress);
			cmd.Parameters.Add("@FirstEmailDate", firstEmailDate);
			cmd.Parameters.Add("@SecondEmailDate", secondEmailDate);
			cmd.Parameters.Add("@DateAdded", dateAdded);
			cmd.Parameters.Add("@CustomerID", j);

			//Open con and execute insert
			AddingToBillsDB(con, cmd);
		}

		private static void AddingToBillsDB(OleDbConnection con, OleDbCommand cmd)
		{
			try
			{
				con.Open();
				//if we affected any rows
				Success(cmd);
				//close connection
				con.Close();
			}
			catch (Exception ex)
			{
				Console.WriteLine(ex.Message);
				//close connection
				con.Close();
			}
		}

		private static void Success(OleDbCommand cmd)
		{
			if (cmd.ExecuteNonQuery() > 0)
			{
				Console.WriteLine("Successfully Inserted to Bills table!");
			}
		}
	}
}
