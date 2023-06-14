namespace Job_Fair_Sys_API.SharedHelper
{
    public static class Utility
    {
        public static string GetStudentPercentile(double cgpa)
        {
            if (cgpa >= 3.5 && cgpa <= 4)
            {
                return "First Teer";
            }
            else if (cgpa >= 3 && cgpa <= 3.5)
            {
                return "Second Teer";
            }
            else if (cgpa < 3)
            {
                return "Third Teer";
            }
            else
            {
                return "cgpa not found";
            }

        }
    }
}