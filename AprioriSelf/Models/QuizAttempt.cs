//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace AprioriSelf.Models
{
    using System;
    using System.Collections.Generic;
    
    public partial class QuizAttempt
    {
        public int AttemptID { get; set; }
        public string Answers { get; set; }
        public double Marks { get; set; }
        public System.DateTime AttemptTime { get; set; }
        public string LearnerID { get; set; }
        public int OfferedCourseID { get; set; }
        public int QuizID { get; set; }
        public string Learner_Username { get; set; }
    
        public virtual OfferedCours OfferedCours { get; set; }
        public virtual Quiz Quiz { get; set; }
        public virtual User User { get; set; }
    }
}
