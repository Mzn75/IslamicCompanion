using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace IslamicCompanion.Models
{
    public class Friendship
    {
        [Key]
        public int Id { get; set; }

        // The person who sent the friend request
        public int RequesterId { get; set; }
        [ForeignKey("RequesterId")]
        public AppUser Requester { get; set; }

        // The person receiving the request
        public int ReceiverId { get; set; }
        [ForeignKey("ReceiverId")]
        public AppUser Receiver { get; set; }

        // Status: 0 = Pending, 1 = Accepted
        public int Status { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.Now;
    }
}
