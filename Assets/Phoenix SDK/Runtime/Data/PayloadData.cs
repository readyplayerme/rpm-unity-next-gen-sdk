namespace  ReadyPlayerMe
{
    public struct PayloadData
    {
        public IRequest data { get; set; }
        public string type;
        public string message;
        public int status;
    }
}
