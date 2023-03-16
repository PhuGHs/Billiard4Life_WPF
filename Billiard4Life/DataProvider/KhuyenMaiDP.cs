using Billiard4Life.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Billiard4Life.DataProvider
{
    public class KhuyenMaiDP : DataProvider
    {
        private static KhuyenMaiDP flag;
        public static KhuyenMaiDP Flag
        {
            get
            {
                if (flag == null) flag = new KhuyenMaiDP();
                return flag;
            }
            set
            {
                flag = value;
            }
        }

        public ObservableCollection<KhuyenMai> GetKhuyenMais()
        {
            ObservableCollection<KhuyenMai> KhuyenMais = new ObservableCollection<KhuyenMai>();
            try
            {
                DataTable dt = LoadInitialData("Select * from KHUYENMAI");
                foreach (DataRow dr in dt.Rows)
                {
                    string? makm = Convert.ToString(dr["MaKM"]);
                    string? tenkm = Convert.ToString(dr["TenKM"]);
                    int giamgia = Convert.ToInt32(dr["GiamGia"]);
                    Decimal mucapdung = Convert.ToDecimal(dr["MucApDung"]);
                    DateTime ngaybd = Convert.ToDateTime(dr["BatDau"]);
                    DateTime ngaykt = Convert.ToDateTime(dr["KetThuc"]);
                    string? mota = Convert.ToString(dr["MoTa"]);

                    if (makm == null || tenkm == null || mota == null)
                    {
                        throw new ArgumentNullException("Thieu property khong the lay len tu database");
                    }
                    KhuyenMais.Add(new KhuyenMai(makm, tenkm, giamgia, mucapdung, ngaybd, ngaykt, mota));
                }
            }
            catch (Exception ex)
            {
                MyMessageBox msb = new MyMessageBox(ex.Message);
                msb.Show();
            }
            finally
            {
                DBClose();
            }
            return KhuyenMais;
        }
    }
}
