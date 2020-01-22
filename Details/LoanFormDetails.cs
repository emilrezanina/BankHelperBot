namespace BankHelperBot.Details
{
    public class LoanFormDetails
    {
        public int Id { get; set; }
        public string MaritalStatus { get; set; }
        public int? ChildCount { get; set; }
        public bool? WorkStatus { get; set; }
        public int? Income { get; set; }
        public bool? Debt { get; set; }

        public LoanFormDetails()
        {
            Id = _idCounter;
        }

        private static int _idCounter = 1000;
        
        public static int IdCounter => _idCounter++;
    }
}