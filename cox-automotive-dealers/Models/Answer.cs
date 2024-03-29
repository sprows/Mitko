using System;
using System.Linq;
using System.Text;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.ComponentModel.DataAnnotations;

using Newtonsoft.Json;

namespace CoxAutomotive.ProgrammingChallenge.Models
{
    /// <summary>
    /// Answer
    /// </summary>
    [DataContract]
    public class Answer :  IEquatable<Answer>, IValidatableObject
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Answer" /> class.
        /// </summary>
        /// <param name="dealers">dealers.</param>
        public Answer(List<DealerAnswer> dealers = default(List<DealerAnswer>))
        {
            this.Dealers = dealers;
        }
        
        /// <summary>
        /// Gets or Sets Dealers
        /// </summary>
        [DataMember(Name="dealers", EmitDefaultValue=false)]
        public List<DealerAnswer> Dealers { get; set; }

        /// <summary>
        /// Returns the string presentation of the object
        /// </summary>
        /// <returns>String presentation of the object</returns>
        public override string ToString()
        {
            var sb = new StringBuilder();
            sb.Append("class Answer {\n");
            sb.Append("  Dealers: ").Append(Dealers).Append("\n");
            sb.Append("}\n");
            return sb.ToString();
        }
  
        /// <summary>
        /// Returns the JSON string presentation of the object
        /// </summary>
        /// <returns>JSON string presentation of the object</returns>
        public virtual string ToJson()
        {
            return JsonConvert.SerializeObject(this, Formatting.Indented);
        }

        /// <summary>
        /// Returns true if objects are equal
        /// </summary>
        /// <param name="input">Object to be compared</param>
        /// <returns>Boolean</returns>
        public override bool Equals(object input)
        {
            return this.Equals(input as Answer);
        }

        /// <summary>
        /// Returns true if Answer instances are equal
        /// </summary>
        /// <param name="input">Instance of Answer to be compared</param>
        /// <returns>Boolean</returns>
        public bool Equals(Answer input)
        {
            if (input == null)
                return false;

            return 
                (
                    this.Dealers == input.Dealers ||
                    this.Dealers != null &&
                    this.Dealers.SequenceEqual(input.Dealers)
                );
        }

        /// <summary>
        /// Gets the hash code
        /// </summary>
        /// <returns>Hash code</returns>
        public override int GetHashCode()
        {
            unchecked // Overflow is fine, just wrap
            {
                int hashCode = 41;
                if (this.Dealers != null)
                    hashCode = hashCode * 59 + this.Dealers.GetHashCode();
                return hashCode;
            }
        }

        /// <summary>
        /// To validate all properties of the instance
        /// </summary>
        /// <param name="validationContext">Validation context</param>
        /// <returns>Validation Result</returns>
        IEnumerable<System.ComponentModel.DataAnnotations.ValidationResult> IValidatableObject.Validate(ValidationContext validationContext)
        {
            yield break;
        }
    }

}
