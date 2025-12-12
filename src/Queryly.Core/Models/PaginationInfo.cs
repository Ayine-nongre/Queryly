public class PaginationInfo
{
    public int PageNumber { get; set; }
    public int PageSize { get; set; }
    public int TotalPages { get; set; }
    public int TotalRows { get; set; }
    public bool HasPreviousPage => PageNumber > 1;
    public bool HasNextPage => PageNumber < TotalPages;

    public PaginationInfo(int pageNumber, int pageSize)
    {
        PageNumber = pageNumber;
        PageSize = pageSize;
    }

    public static void NextPage(PaginationInfo pageInfo)
    {
        if (pageInfo.HasNextPage)
        {
            pageInfo.PageNumber++;
        }
    }

    public static void PreviousPage(PaginationInfo pageInfo)
    {
        if (pageInfo.HasPreviousPage)
        {
            pageInfo.PageNumber--;
        }
    }

    public void GoToPage(int pageNumber)
    {
        if (pageNumber >= 1 && pageNumber <= TotalPages)
        {
            PageNumber = pageNumber;
        }
    }
}