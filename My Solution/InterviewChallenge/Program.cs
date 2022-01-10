using System;
using System.Collections.Generic;
using System.Data;
using System.Data.OleDb;
using System.IO;
using System.Linq;
using System.Xml;
using System.Xml.Linq;

namespace InterviewChallenge
{
	//Possible TODOS in future
	//Fix the "Jann" typo from XML sheet in program
	//Fix the way that Customer_ID is mapped to my db(By checkng if the Customer ID is alreadey there)
	//Fix file paths to be dynamicallly updated 
	class Program
	{
		static void Main(string[] args)
		{
			//-------------Part 1-------------//
			populateRptFile();

			//-------------Part 2-------------//
			PopulateDB();

			//-------------Part 3-------------//
			PopulateCSV();
		}

		//Will populate rpt file
		private static void populateRptFile()
		{
			//Will save to netcoreapp3.1
			FileStream ostrm;
			StreamWriter writer;
			TextWriter oldOut = Console.Out;
			try
			{
				ostrm = new FileStream("./BillFile-mmddyyyy.rpt", FileMode.OpenOrCreate, FileAccess.Write);
				writer = new StreamWriter(ostrm);
			}
			catch (Exception e)
			{
				Console.WriteLine("Cannot open BillFile-mmddyyyy.rpt for writing");
				Console.WriteLine(e.Message);
				return;
			}
			Console.SetOut(writer);

			//Call helper
			LoadBill();

			Console.SetOut(oldOut);
			writer.Close();
			ostrm.Close();
			//Feedback
			Console.WriteLine("RPT file Created");
		}

		//Part of (populater RPT)
		static public void LoadBill()
		{
			double INVOICE_RECORD_TOTAL_AMOUNT = 0;
			int INVOICE_RECORD_COUNT = 0;

			//Using oldway with XmlDocument
			XmlDocument doc = new XmlDocument();
			doc.Load(@"C:\SPECIFY FILEPATH");

			//Using read method to parse and display FR part
			XmlTextReader reader = new XmlTextReader(@"C:\SPECIFY FILEPATH");
			while (reader.Read())
			{
				if (reader.NodeType == XmlNodeType.Element)
				{
					switch (reader.Name)
					{
						case "Bill_Amount":
							INVOICE_RECORD_TOTAL_AMOUNT += XmlConvert.ToDouble(reader.ReadInnerXml());
							INVOICE_RECORD_COUNT++;
							break;
					}
				}
			}
			reader.Close();
			DisplayFRLine(INVOICE_RECORD_COUNT, INVOICE_RECORD_TOTAL_AMOUNT);

			//Displaying AA && HH part
			//Using Linq method XElement
			var billingData = XElement.Load(@"C:\SPECIFY FILEPATH");
			var billerInfo = billingData.Descendants("BILL_HEADER");
			foreach (var field in billerInfo)
			{
				//Create object
				Customer Customer = new Customer();

				Customer.Invoice_No = field.Element("Invoice_No").Value;
				Customer.Account_No = field.Element("Account_No").Value;
				Customer.Customer_Name = field.Element("Customer_Name").Value;
				Customer.Cycle_Cd = field.Element("Cycle_Cd").Value;
				Customer.Bill_Dt = field.Element("Bill_Dt").Value;
				Customer.Due_Dt = field.Element("Due_Dt").Value;
				Customer.Account_Class = field.Element("Account_Class").Value;

				//Bill Elements
				var bill = field.Descendants("Bill");
				foreach (var billInfo in bill)
				{
					Customer.Bill_Amount = billInfo.Element("Bill_Amount").Value;
					Customer.Balance_Due = billInfo.Element("Balance_Due").Value;
					Customer.Bill_Run_Dt = billInfo.Element("Bill_Run_Dt").Value;
					Customer.Bill_Run_Seq = billInfo.Element("Bill_Run_Seq").Value;
					Customer.Bill_Run_Tm = billInfo.Element("Bill_Run_Tm").Value;
					Customer.Bill_Tp = billInfo.Element("Bill_Tp").Value;
				}

				//Address Elements
				var address = field.Descendants("Address_Information");
				foreach (var addressInfo in address)
				{
					Customer.Mailing_Address_1 = addressInfo.Element("Mailing_Address_1").Value;
					Customer.Mailing_Address_2 = addressInfo.Element("Mailing_Address_2").Value;
					Customer.City = addressInfo.Element("City").Value;
					Customer.State = addressInfo.Element("State").Value;
					Customer.Zip = addressInfo.Element("Zip").Value;
				}

				Customer.DisplayAAInfo();
			}
		}

		//printing out the FR Line (part of LoadBill() )
		static public void DisplayFRLine(int IHcount, double sumAmount)
		{
			const string clientGUID = "8203ACC7-2094-43CC-8F7A-B8F19AA9BDA2";
			string fileName = Path.GetFileNameWithoutExtension(@"C:\SPECIFY FILEPATH");

			Console.Write("1~FR");
			Console.Write("|2~" + clientGUID);
			Console.Write("|3~" + fileName);//assuming "Sample UT file" is file name of where I got all info.
			string datestring = DateTime.Now.ToString("MM/dd/yyyy");
			Console.Write("|4~" + datestring);
			Console.Write("|5~" + IHcount);
			Console.WriteLine("|6~" + String.Format("{0:0.00}", sumAmount));
		}

		//Will populate database from rpt file that was created previously
		private static void PopulateDB()
		{
			//Go through each element in rpt file and store in class
			string filePath = @"C:\SPECIFY FILEPATH";
			List<string> lines = File.ReadAllLines(filePath).ToList();
			List<Customer> customer = new List<Customer>();

			//Todays current date for dateAdded
			string datestring = DateTime.Now.ToString("MM/dd/yyyy");

			//Find GUID since it's in the first line and only shows once.
			string[] entries = lines[0].Split('|');
			string formatGUID = Array.Find(entries, element => element.StartsWith("2~"))?.Remove(0,2);

			int j = 1;

			//Get AA and HH Fields
			for (int i = 1; i < lines.Capacity; i = (i+2))
			{
				BillOfCustomer bill = new BillOfCustomer();

				//Getting  aa and hh line
				string[] AALine = lines[i].Split('|');
				string[] HHLine = lines[i+1].Split('|');


				//Search AA line and assign
				bill.name = Array.Find(AALine, element => element.StartsWith("VV~"))?.Remove(0, 3);
				bill.acctNum = Array.Find(AALine, element => element.StartsWith("BB~"))?.Remove(0, 3);
				bill.addr = Array.Find(AALine, element => element.StartsWith("CC~"))?.Remove(0, 3);
				bill.city = Array.Find(AALine, element => element.StartsWith("EE~"))?.Remove(0, 3);
				bill.state = Array.Find(AALine, element => element.StartsWith("FF~"))?.Remove(0, 3);
				bill.zip = Array.Find(AALine, element => element.StartsWith("GG~"))?.Remove(0, 3);
				bill.dateAdded = datestring;

				//Search HH Line and assign
				bill.billDt = Array.Find(HHLine, element => element.StartsWith("LL~"))?.Remove(0, 3);
				bill.billNum = Array.Find(HHLine, element => element.StartsWith("KK~"))?.Remove(0, 3);
				bill.billAmnt = Array.Find(HHLine, element => element.StartsWith("NN~"))?.Remove(0, 3);
				bill.formatGUID = formatGUID;
				bill.accntBalance = Array.Find(HHLine, element => element.StartsWith("QQ~"))?.Remove(0, 3);
				bill.dueDate = Array.Find(HHLine, element => element.StartsWith("MM~"))?.Remove(0, 3);
				bill.serviceAddress = Array.Find(HHLine, element => element.StartsWith("SS~"))?.Remove(0, 3);
				bill.firstEmailDate = Array.Find(HHLine, element => element.StartsWith("OO~"))?.Remove(0, 3);
				bill.secondEmailDate = Array.Find(HHLine, element => element.StartsWith("PP~"))?.Remove(0, 3);
				bill.dateAdded = datestring;
				

				//Adding customer to Bills table
				bill.addToBillsTable(j);
				j++;

				//Adding customer to Customer table
				bill.addToCustomerTable();
			}
		}

		//Will populate the CSV
		private static void PopulateCSV()
		{
			//connect to my DB && get customer table in Data table
			string conString = @"Provider=Microsoft.ACE.OLEDB.12.0;Data Source=C:\SPECIFY FILEPATH;Persist Security Info = False; ";
			OleDbConnection con = new OleDbConnection(conString);
			con.Open();
			OleDbCommand command = new OleDbCommand();
			command.Connection = con;
			string query = "SELECT * FROM Customer";
			command.CommandText = query;
			OleDbDataAdapter da = new OleDbDataAdapter(command);

			//Create datatable to get Customer table
			DataTable custDT = new DataTable();
			da.Fill(custDT);

			//Close connection
			con.Close();



			//connect to my DB && get bills table in Data table
			con.Open();
			command.Connection = con;
			query = "SELECT * FROM Bills";
			command.CommandText = query;
			da = new OleDbDataAdapter(command);

			//Create datatable to get Bills table
			DataTable billsDT = new DataTable();
			da.Fill(billsDT);

			//Close connection
			con.Close();



			//Try to merge the two...addding columns from bills table to customers
			custDT.Columns.Add("Bills.ID");
			custDT.Columns.Add("BillDate");
			custDT.Columns.Add("BillNumber");
			custDT.Columns.Add("AccountBalance");
			custDT.Columns.Add("DueDate");
			custDT.Columns.Add("BillAmount");
			custDT.Columns.Add("FormatGUID");


			//updating the rows in "merged" table
			for (int i = 0; i < custDT.Rows.Count; i++)
			{
				//Inserting Due date without time stamp
				string dueDateIn = (billsDT.Rows[i]["DueDate"]).ToString();
				custDT.Rows[i]["DueDate"] = dueDateIn.Substring(0, dueDateIn.Length - 12);
				//Inserting Bill date without time stamp
				string billDateIn = (billsDT.Rows[i]["BillDate"]).ToString();
				custDT.Rows[i]["BillDate"] = billDateIn.Substring(0, billDateIn.Length - 12);

				custDT.Rows[i]["Bills.ID"] = billsDT.Rows[i]["ID"];
				custDT.Rows[i]["BillNumber"] = billsDT.Rows[i]["BillNumber"];
				custDT.Rows[i]["AccountBalance"] = billsDT.Rows[i]["AccountBalance"];
				custDT.Rows[i]["BillAmount"] = billsDT.Rows[i]["BillAmount"];
				custDT.Rows[i]["FormatGUID"] = billsDT.Rows[i]["FormatGUID"];
			}

			//Change date time order to last column
			custDT.Columns["DateAdded"].SetOrdinal(14);

			//Change name of each column associated with BillingReport.txt
			custDT.Columns[0].ColumnName = "Customer.ID";
			custDT.Columns[1].ColumnName = "Customer.CustomerName";
			custDT.Columns[2].ColumnName = "Customer.AccountNumber";
			custDT.Columns[3].ColumnName = "Customer.CustomerAddress";
			custDT.Columns[4].ColumnName = "Customer.CustomerCity";
			custDT.Columns[5].ColumnName = "Customer.CustomerState";
			custDT.Columns[6].ColumnName = "Customer.CustomerZip";
			custDT.Columns[7].ColumnName = "Bills.ID";
			custDT.Columns[8].ColumnName = "Bills.BillDate";
			custDT.Columns[9].ColumnName = "Bills.BillNumber";
			custDT.Columns[10].ColumnName = "Bills.AccountBalance";
			custDT.Columns[11].ColumnName = "Bills.DueDate";
			custDT.Columns[12].ColumnName = "Bills.BillAmount";
			custDT.Columns[13].ColumnName = "Bills.FormatGUID";
			custDT.Columns[14].ColumnName = "Customer.DateAdded";

			//Exporting the DataTable to CSV using streamwriter
			StreamWriter sw = new StreamWriter("NachoBillingReport.txt");
			//Header
			for (int i = 0; i < custDT.Columns.Count; i++)
			{
				sw.Write(custDT.Columns[i]);
				if (i < custDT.Columns.Count - 1)
				{
					sw.Write(",");
				}
			}
			//For each row
			sw.Write(sw.NewLine);
			PerRowCSV(custDT, sw);
			sw.Close();

			Console.WriteLine("CSV Created!");
		}

		private static void PerRowCSV(DataTable custDT, StreamWriter sw)
		{
			foreach (DataRow dr in custDT.Rows)
			{
				for (int i = 0; i < custDT.Columns.Count; i++)
				{
					if (!Convert.IsDBNull(dr[i]))
					{
						string value = dr[i].ToString();
						//If the value is ',', we want to include (REMOVE FOR .TXT DOC TO GET RID OF QUTES ON NAME)
						if (value.Contains(','))
						{
							value = String.Format("\"{0}\"", value);
							sw.Write(value);
						}
						//if the value has time after date, we remove
						else if (value.Contains("12:00"))
						{
							value = value.Remove(9);
							sw.Write(value);
						}
						else
						{
							sw.Write(dr[i].ToString());
						}
					}
					//CSV part
					if (i < custDT.Columns.Count - 1)
					{
						sw.Write(",");
					}
				}
				sw.Write(sw.NewLine);
			}
		}
	}
}
