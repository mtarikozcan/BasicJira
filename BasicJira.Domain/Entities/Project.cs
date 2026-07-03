using System;
using System.Collections.Generic;
using System.Text;

namespace BasicJira.Domain.Entities;

/*
 table -> id pk, name-string, description-string, startdate-datetime, enddate-datetime, status, createdat-datetime.  
 */

public class Project
{ 
    public Guid Id { get; set; }

    public string Name { get; set; }
    public string Description { get; set; }
    

}
