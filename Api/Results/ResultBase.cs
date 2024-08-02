namespace Api.Results;

public class ResultBase
{
    public bool Ok { get; set; }
    public string? Msg { get; set; }
    public int StatusCode { get; set; }
    public void setConfirm(string msg){
        Ok = true;
        Msg = msg;
        StatusCode = 200;
    }

    public void setError(string errorMsg){
        Ok = false;
        Msg = errorMsg;
    }
}
