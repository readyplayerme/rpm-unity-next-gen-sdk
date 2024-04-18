using System.Collections.Generic;

namespace ReadyPlayerMe
{
    public class CharactersRequest : IRequest
    {
        public string organizationId { get; set; }
        public Dictionary<string, string> assets { get; set; }
    }
}
