using System.Threading.Tasks;

namespace ReadyPlayerMe
{
    public class EndpointBase
    {
        public async virtual Task<IResponse> Get() { return null; }
        public async virtual Task<IResponse> Get(string parameter) { return null; }
        public async virtual Task<IResponse> Post(IRequest request) { return null; }
        public async virtual Task<IResponse> Put(string parameter, IRequest request) { return null; }
        public async virtual Task<IResponse> Delete(string parameter, IRequest request) { return null; }
        public async virtual Task<IResponse> Patch(string parameter, IRequest request) { return null; }
    }
}
