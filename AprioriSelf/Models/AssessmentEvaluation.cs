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
    
    public partial class AssessmentEvaluation
    {
        public int AssessmentID { get; set; }
        public int LearnerEnrollID { get; set; }
        public int Points { get; set; }
        public string Feedback { get; set; }
        public System.DateTime EvaluationDate { get; set; }
        public byte[] File { get; set; }
    
        public virtual Assessment Assessment { get; set; }
        public virtual LearnerEnroll LearnerEnroll { get; set; }
    }
}