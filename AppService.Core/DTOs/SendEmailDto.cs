namespace AppService.Core.DTOs;

public class SendEmailDto
{


    public string from { get; set; }
    public string subjectEmail { get; set; }
    public string sendTo { get; set; }
    public string bodyText { get; set; }
    public string template { get; set; }

    
  
}



public class ParamsTemplate
{

    
    public string name { get; set; }
    
  
}
