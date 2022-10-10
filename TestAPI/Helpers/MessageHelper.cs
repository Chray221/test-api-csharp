using System.Net;

namespace TestAPI.Helpers
{
    public static class MessageExtension
    {
        /// <summary>
        /// returns:
        /// <para> {</para>
        /// <para>      "custom_message": { </para>
        /// <para>      "title": "Title Here", </para>
        /// <para>      "content": "Message Here", </para>
        /// <para>      "button": "Button Text Here", </para>
        /// <para>      "icon": "fontawesome icon name here", </para>
        /// <para>      "icon_type": 0 </para>
        /// <para>  } </para>
        /// <para>  "statusCode": 0, </para>
        /// <para>  } </para>
        /// 
        /// </summary>
        /// <param name="title"></param>
        /// <param name="content"></param>
        /// <param name="button"></param>
        /// <param name="icon"></param>
        /// <param name="icon_type">
        /// VALUES:
        /// <para>0 - no icon</para>
        /// <para>1 - FontAwesome Font - brand</para>
        /// <para>2 - FontAwesome Font - regular</para>
        /// <para>3 - FontAwesome Font - solid</para>
        /// </param>
        /// <param name="statusCode"></param>
        /// <returns></returns>
        public static object ShowCustomMessage(string title, string content, string button = "Okay", string icon = null, int icon_type = 0, HttpStatusCode statusCode = HttpStatusCode.OK)
        {
            //status = 200 default
            return new { custom_message = new { title, icon, icon_type, content, button }/*,status = statusCode */};
        }

        public static object ShowRequiredMessage(string propertyName)
        {
            return new { error = $"{propertyName} is required!." /*,status = HttpStatusCode.UnprocessableEntity*/ };
        }
    }
}
