
namespace Kuci.Models
{
    public interface IModelsHolder
    {
        public T Get<T>() where T : ModelBase;
    }
}
