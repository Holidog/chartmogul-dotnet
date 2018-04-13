namespace chartmoguldotnet
{
    public class Payment : AbstractTransaction
    {
        public new string Type
        { get { return "payment"; } }
    }
}
