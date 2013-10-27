
namespace BomberEngine
{
    public abstract class BaseElement : BaseObject, IUpdatable, IDrawable, IEventHandler
    {
        public int id;

        public virtual void Update(float delta)
        {   
        }

        public virtual void Draw(Context context)
        {
        }

        public virtual bool HandleEvent(Event evt)
        {
            return false;
        }
    }
}
