/// <summary>
/// Summary description for IListAction
/// </summary>
namespace Roberta.WebControls
{
    public interface IListAction
    {
        ListActionLink GetAction(ListActionType listAction);
        
    }
}
