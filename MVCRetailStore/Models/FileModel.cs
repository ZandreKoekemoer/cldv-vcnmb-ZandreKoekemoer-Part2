namespace MVCRetailStore.Models
{
    public class FileModel
    {
        public string Name { get; set; }
        public long Size { get; set; }
        public DateTimeOffset? LastModified { get; set; }

        public string DisplaySize
        {
            get
            {
                if (Size >= 1024 * 1024)
                {
                    return $"{Size / 1024 / 1024} MB";
                }
                if (Size >= 1024)
                {
                    return $"{Size / 1024} KB";
                }
                return $"{Size} Bytes";
            }
        }

    }
}
/*
Reece Waving. 2025. CLDV6212 ASP.NET MVC & Azure Series - Part 2: Adding Image Uploads with Blob Storage! (Version 2.0) [Source code].
Available at: <https://youtu.be/CuszKqZvRuM?si=lTfJaqI02wmHcIkh>
[Accessed 26 August 2025].


Reece Waving. 2025. CLDV6212 ASP.NET MVC & Azure Series - Part 4: Mastering Azure File Share! (Version 2.0) [Source code].
Available at: <https://youtu.be/A-mVVL88oEg?si=sUYFyrY2wQc6Lny0>
[Accessed 26 August 2025].

/*
Bootstrap. 2023. Bootstrap 5 Documentation.
Available at:
<https: //getbootstrap.com/docs/5.3/getting-started/introduction />
[Accessed 28 August 2025].

Stack Overflow. 2015. Calculate price based on input number (quantity) change.  
Available at: <https://stackoverflow.com/questions/27764823/calculate-price-based-on-input-number-quantity-change>  
[Accessed 28 August 2025].

*/
