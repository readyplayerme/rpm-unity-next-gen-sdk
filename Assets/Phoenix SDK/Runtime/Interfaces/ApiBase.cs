using System.Threading.Tasks;

namespace ReadyPlayerMe
{
    public class ApiBase<T>
    {
        public async virtual Task<T> Get() { return default; }
        public async virtual Task<T> Get(string parameter) { return default; }
        public async virtual Task<T> Post(IRequest request) { return default; }
        public async virtual Task<T> Put(string parameter, IRequest request) { return default; }
        public async virtual Task<T> Delete(string parameter, IRequest request) { return default; }
        public async virtual Task<T> Patch(string parameter, IRequest request) { return default; }
    }
}
