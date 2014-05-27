using Split_It_.Model;
using Split_It_.Request;
using Split_It_.Utils;
using SQLite;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace Split_It_.Controller
{
    public class CommentDatabase
    {
        Action<List<Comment>> callback;

        public CommentDatabase(Action<List<Comment>> callback)
        {
            this.callback = callback;
        }

        public void getComments(int expenseId)
        {
            GetCommentsRequest request = new GetCommentsRequest(expenseId);
            request.getComments(callback);
        }

        public void addComment()
        {

        }
    }
}
