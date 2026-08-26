
public class ItemControl_Online_TowerMode: ItemControl_Online
{

    protected override IItemPathProvider CreateItemPathProvider(){
        return new ItemPathProvider_Tower();
    }

}
