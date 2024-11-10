
public class BomConfigurationBase
{
    protected object value;

    public virtual void Request(){}

    public void Set(object newValue)
    {
        value = newValue;
    }
    public object Get()
    {
        return value;
    }
}
