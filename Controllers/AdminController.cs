﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Final_APP.Entities;
using System.Security.Cryptography;
using System.Text;
using System.Net;
using System.Data.Entity;
namespace Final_APP.Controllers
{
    public class AdminController : Controller
    {
        private DVMB conn = new DVMB();
        // GET: Admin
        public ActionResult Index()

        {
            if (Session["Admin"] != null)
            {
                return View();
            }
            else
            {
                return RedirectToAction("Login", "Login");
            }
        }
        //Quan li nguoi dung 

        //Thong tin tai khona khach hang
        public ActionResult UserAccount()
        {
            List<TaiKhoan> ketqua = conn.TaiKhoans.ToList();
            return View(ketqua);
        }

        //----------------------end thong tin tai khoan

        //Xoa tai khoan
        public ActionResult DeleteAccount(string TenDangNhap)
        {
            if (TenDangNhap == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            TaiKhoan taiKhoan = conn.TaiKhoans.Find(TenDangNhap);
            if (taiKhoan == null)
            {
                return HttpNotFound();
            }
            return View(taiKhoan);
        }

        // POST: TaiKhoan/Delete/5
        [HttpPost, ActionName("DeleteAccount")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteAccountConfirmed(string TenDangNhap)
        {


            conn.TaiKhoans.SqlQuery("ALTER TABLE Taikhoan NOCHECK CONSTRAINT ALL;");
            conn.TaiKhoans.SqlQuery("ALTER TABLE NGUOIDUNG NOCHECK CONSTRAINT ALL;");
            TaiKhoan taiKhoan = conn.TaiKhoans.Find(TenDangNhap);
            conn.TaiKhoans.Remove(taiKhoan);
            conn.SaveChanges();
            conn.TaiKhoans.SqlQuery("ALTER TABLE TAIKHOAN CHECK CONSTRAINT ALL");
            conn.TaiKhoans.SqlQuery("ALTER TABLE NGUOIDUNG NOCHECK CONSTRAINT ALL;");
            return RedirectToAction("UserAccount");
        }

        //------------End xoa tai khoan



        //Them nguoi dung
        public ActionResult AddUser()
        {
            return View();
        }


        // POST:NguoiDung/Create

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult AddUser(User_Account user_Account)
        {
            
            var taiKhoan = new TaiKhoan()
            {
                TenDangNhap = user_Account.TenDangNhap,
                MatKhau = user_Account.MatKhau,
                Quyen = user_Account.Quyen =false,
            };

            var nguoiDung = new NguoiDung()
            {
                MaNguoiDung = user_Account.MaNguoiDung,
                HoTen = user_Account.HoTen,
                SDT = user_Account.SDT,
                Email = user_Account.Email,
                TenDangNhap = user_Account.TenDangNhap,
            };
            
            conn.TaiKhoans.Add(taiKhoan);
            conn.NguoiDungs.Add(nguoiDung);
            conn.SaveChanges();
            return RedirectToAction("AddUser");
        }
        //-----------End them nguoi dung




        //Thong tin nguoi dung
        public ActionResult UserDetail()
        {
            List<NguoiDung> ketqua = conn.NguoiDungs.ToList();
            List<TaiKhoan> account = conn.TaiKhoans.ToList();
            return View(ketqua);
        }
        [HttpPost]
        public ActionResult UserDetail(string mand, string hoten, string sdt, string email)
        {
            ViewBag.mand = mand;
            ViewBag.hoten = hoten;
            ViewBag.sdt = sdt;
            ViewBag.email = email;
            List<NguoiDung> nd = conn.NguoiDungs.ToList();
            var NguoiDung = (from n in nd
                             where (n.MaNguoiDung.Contains(mand))
                             where (n.HoTen.Contains(hoten))
                             where (n.SDT.Contains(sdt))
                             where (n.Email.Contains(email))
                             select new NguoiDung
                             {
                                 MaNguoiDung = n.MaNguoiDung,
                                 HoTen = n.HoTen,
                                 SDT = n.SDT,
                                 Email = n.Email,
                             }).OrderBy(x => x.MaNguoiDung).ToList();
            return View(NguoiDung);

        }
        //Sua thong tin nguoi dung
        // GET: NguoiDungs/Edit
        public ActionResult EditUser(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            NguoiDung nguoiDung = conn.NguoiDungs.Find(id);
            if (nguoiDung == null)
            {
                return HttpNotFound();
            }
            ViewBag.TenDangNhap = new SelectList(conn.TaiKhoans, "TenDangNhap", "MatKhau", nguoiDung.TenDangNhap);
            return View(nguoiDung);
        }

        // POST: NguoiDungs/Edit
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult EditUser([Bind(Include = "MaNguoiDung,HoTen,SDT,Email,TenDangNhap")] NguoiDung nguoiDung)
        {
            if (ModelState.IsValid)
            {
                conn.Entry(nguoiDung).State = EntityState.Modified;
                conn.SaveChanges();
                return RedirectToAction("UserDetail");
            }
            ViewBag.TenDangNhap = new SelectList(conn.TaiKhoans, "TenDangNhap", "MatKhau", nguoiDung.TenDangNhap);
            return View(nguoiDung);
        }



        //---------------End quan li nguoi dung

        //-----------------Lich su dat ve

        public ActionResult LichSuDatVe()
        {
            List<LichSuDatVe> lichSuDatVe = new List<LichSuDatVe>();
            List<NguoiDung> nguoidung = conn.NguoiDungs.ToList();
            List<PhieuDatVe> phieudatve = conn.PhieuDatVes.ToList();
            List<HoaDon> hoadon = conn.HoaDons.ToList();
            var LichSuDatVe =  (from n in nguoidung
                         join p in phieudatve on n.MaNguoiDung equals p.MaNguoiDung
                         join h in hoadon on p.MaPhieuDatVe equals h.MaPhieuDatVe
                         select new LichSuDatVe
                         {
                             MaNguoiDung = n.MaNguoiDung,
                             HoTen = n.HoTen,
                             NgayDat = p.NgayDat,
                             ThanhTien = h.ThanhTien,
                            
                         }).OrderBy(x => x.NgayDat);
            lichSuDatVe = LichSuDatVe.ToList();
            return View(lichSuDatVe);
        }
       [HttpPost]
        public ActionResult LichSuDatVe(string mand, string hoten, string thoigian, string gia)
        {
            ViewBag.mand = mand;
            ViewBag.hoten = hoten;
            ViewBag.thoigian = thoigian;
            ViewBag.gia = gia;
            if (thoigian == "12:00:00 AM")
            {
                thoigian = "no";
            }
            else
            {
                thoigian = thoigian.Replace("12:00:00 AM", "");
            }
            
            List<LichSuDatVe> lichSuDatVe = new List<LichSuDatVe>();
            List<NguoiDung> nguoidung = conn.NguoiDungs.ToList();
            List<PhieuDatVe> phieudatve = conn.PhieuDatVes.ToList();
            List<HoaDon> hoadon = conn.HoaDons.ToList();
            var LichSuDatVe = (from n in nguoidung
                              // where (n.MaNguoiDung.Contains(mand))
                               join p in phieudatve on n.MaNguoiDung equals p.MaNguoiDung
                               join h in hoadon on p.MaPhieuDatVe equals h.MaPhieuDatVe
                               where (n.MaNguoiDung.Contains(mand))
                               where (n.HoTen.Contains(hoten))
                               where (p.NgayDat.ToString().Contains(thoigian))
                               where (h.ThanhTien.ToString().Contains(gia))
                               select new LichSuDatVe
                               {
                                   MaNguoiDung = n.MaNguoiDung,
                                   HoTen = n.HoTen,
                                   NgayDat = p.NgayDat,
                                   ThanhTien = h.ThanhTien,

                               }).OrderBy(x => x.NgayDat);
            lichSuDatVe = LichSuDatVe.ToList();
            
           // ViewBag.
            return View(lichSuDatVe);
        }

        //-----------------Lich su dat ve end

        // Quan li chuyen bay start
        // GET: ChangBays
        public ActionResult IndexChangBay()
        {
            var changBays = conn.ChangBays.Include(c => c.SanBay).Include(c => c.SanBay1);

            var sanbay = (from s in conn.SanBays select s).ToList();

            ViewBag.SanBay = sanbay;

            return View(changBays.ToList());
        }

        // GET: ChangBays/Edit/5
        public ActionResult EditChangBay(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ChangBay changBay = conn.ChangBays.Find(id);
            if (changBay == null)
            {
                return HttpNotFound();
            }
            ViewBag.SanBay_CatCanh = new SelectList(conn.SanBays, "MaSanBay", "TenSanBay", changBay.SanBay_CatCanh);
            ViewBag.SanBay_HaCanh = new SelectList(conn.SanBays, "MaSanBay", "TenSanBay", changBay.SanBay_HaCanh);
            return View(changBay);
        }

        // POST: ChangBays/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult EditChangBay([Bind(Include = "MaChangBay,SanBay_CatCanh,SanBay_HaCanh")] ChangBay changBay)
        {
            if (ModelState.IsValid)
            {
                conn.Entry(changBay).State = EntityState.Modified;
                conn.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.SanBay_CatCanh = new SelectList(conn.SanBays, "MaSanBay", "TenSanBay", changBay.SanBay_CatCanh);
            ViewBag.SanBay_HaCanh = new SelectList(conn.SanBays, "MaSanBay", "TenSanBay", changBay.SanBay_HaCanh);
            return View(changBay);
        }
        // GET: ChangBays/Create


        public ActionResult CreateChangBay()
        {
            ViewBag.SanBay_CatCanh = new SelectList(conn.SanBays, "MaSanBay", "TenSanBay");
            ViewBag.SanBay_HaCanh = new SelectList(conn.SanBays, "MaSanBay", "TenSanBay");
            return View();
        }

        // POST: ChangBays/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult CreateChangBay([Bind(Include = "MaChangBay,SanBay_CatCanh,SanBay_HaCanh")] ChangBay changBay)
        {
            if (ModelState.IsValid)
            {
                conn.ChangBays.Add(changBay);
                conn.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.SanBay_CatCanh = new SelectList(conn.SanBays, "MaSanBay", "TenSanBay", changBay.SanBay_CatCanh);
            ViewBag.SanBay_HaCanh = new SelectList(conn.SanBays, "MaSanBay", "TenSanBay", changBay.SanBay_HaCanh);
            return View(changBay);
        }
        //Chuyen bay ------------
        // GET: ChuyenBays
        public ActionResult IndexChuyenBay()
        {
            var chuyenBays = conn.ChuyenBays.Include(c => c.ChangBay).Include(c => c.MayBay);
            return View(chuyenBays.ToList());
        }

        // GET: ChuyenBays/ChiTiet
        public ActionResult DetailChuyenBay(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ChuyenBay chuyenBay = conn.ChuyenBays.Find(id);
            if (chuyenBay == null)
            {
                return HttpNotFound();
            }
            return View(chuyenBay);
        }

        // GET: ChuyenBays/Edit/5
        public ActionResult EditChuyenBay(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ChuyenBay chuyenBay = conn.ChuyenBays.Find(id);
            if (chuyenBay == null)
            {
                return HttpNotFound();
            }
            ViewBag.MaChangBay = new SelectList(conn.ChangBays, "MaChangBay", "SanBay_CatCanh", chuyenBay.MaChangBay);
            ViewBag.LoaiMayBay = new SelectList(conn.MayBays, "LoaiMayBay", "TenMayBay", chuyenBay.LoaiMayBay);
            return View(chuyenBay);
        }

        // POST: ChuyenBays/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult EditChuyenBay([Bind(Include = "MaChuyenBay,TGbay,NgayBay,GioBay,LoaiMayBay,Gia,SaleTreEm,SaleEmBe,MaChangBay")] ChuyenBay chuyenBay)
        {
            if (ModelState.IsValid)
            {
                conn.Entry(chuyenBay).State = EntityState.Modified;
                conn.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.MaChangBay = new SelectList(conn.ChangBays, "MaChangBay", "SanBay_CatCanh", chuyenBay.MaChangBay);
            ViewBag.LoaiMayBay = new SelectList(conn.MayBays, "LoaiMayBay", "TenMayBay", chuyenBay.LoaiMayBay);
            return View(chuyenBay);
        }

        // GET: ChuyenBays/Create
        public ActionResult CreateChuyenBay()
        {
            ViewBag.MaChangBay = new SelectList(conn.ChangBays, "MaChangBay", "SanBay_CatCanh");
            ViewBag.LoaiMayBay = new SelectList(conn.MayBays, "LoaiMayBay", "TenMayBay");
            return View();
        }

        // POST: ChuyenBays/Create

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult CreateChuyenBay([Bind(Include = "MaChuyenBay,TGbay,NgayBay,GioBay,LoaiMayBay,Gia,SaleTreEm,SaleEmBe,MaChangBay")] ChuyenBay chuyenBay)
        {
            if (ModelState.IsValid)
            {
                conn.ChuyenBays.Add(chuyenBay);
                conn.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.MaChangBay = new SelectList(conn.ChangBays, "MaChangBay", "SanBay_CatCanh", chuyenBay.MaChangBay);
            ViewBag.LoaiMayBay = new SelectList(conn.MayBays, "LoaiMayBay", "TenMayBay", chuyenBay.LoaiMayBay);
            return View(chuyenBay);
        }


        // POST: ChuyenBays/Delete/5
        [HttpPost]

        public ActionResult DeleteChuyenBay(string id)
        {
            try
            {
                using (var conn = new DVMB())
                {
                    var obj = conn.ChuyenBays.Find(id);
                    conn.ChuyenBays.Remove(obj);
                    conn.SaveChanges();
                    return Json(new { message = "Success!" }, JsonRequestBehavior.AllowGet);
                }
            }
            catch (Exception e)
            {
                return Json(new { message = "Fail!" }, JsonRequestBehavior.AllowGet);
            }
        }

        //---------------------End Chuyen Bay

        public ActionResult IndexSanBay()
        {
            return View(conn.SanBays.ToList());
        }

        // GET: SanBays/Create
        public ActionResult CreateSanBay()
        {
            return View();
        }

        // POST: SanBays/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult CreateSanBay([Bind(Include = "MaSanBay,TenSanBay,ViTri,KhuVuc")] SanBay sanBay)
        {
            if (ModelState.IsValid)
            {
                conn.SanBays.Add(sanBay);
                conn.SaveChanges();
                return RedirectToAction("IndexSanBay");
            }

            return View(sanBay);
        }

        // index
        public ActionResult IndexHoaDon()
        {
            var hoaDons = conn.HoaDons.Include(h => h.HinhThucThanhToan).Include(h => h.KhuyenMai).Include(h => h.PhieuDatVe);
            return View(hoaDons.ToList());
        }

        // Chi tiet hoa don

        // GET: HoaDons/Details/5
        public ActionResult DetailHoaDon(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            HoaDon hoaDon = conn.HoaDons.Find(id);
            if (hoaDon == null)
            {
                return HttpNotFound();
            }
            return View(hoaDon);
        }

        //-----------Hoa Don end
        //--------------Ngan hANG

        //index
        public ActionResult IndexNganHang()
        {
            var nganHangs = conn.NganHangs.Include(n => n.HinhThucThanhToan);
            return View(nganHangs.ToList());
        }

        // GET: NganHangs/Edit/5
        public ActionResult EditNganHang(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            NganHang nganHang = conn.NganHangs.Find(id);
            if (nganHang == null)
            {
                return HttpNotFound();
            }
            ViewBag.MaHinhThucTT = new SelectList(conn.HinhThucThanhToans, "MaHinhThucTT", "TenHinhThucTT", nganHang.MaHinhThucTT);
            return View(nganHang);
        }

        // POST: NganHangs/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult EditNganHang([Bind(Include = "MaNganHang,TenNganHang,MaHinhThucTT")] NganHang nganHang)
        {
            if (ModelState.IsValid)
            {
                conn.Entry(nganHang).State = EntityState.Modified;
                conn.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.MaHinhThucTT = new SelectList(conn.HinhThucThanhToans, "MaHinhThucTT", "TenHinhThucTT", nganHang.MaHinhThucTT);
            return View(nganHang);
        }

        // GET: NganHangs/Create
        public ActionResult CreateNganHang()
        {
            ViewBag.MaHinhThucTT = new SelectList(conn.HinhThucThanhToans, "MaHinhThucTT", "TenHinhThucTT");
            return View();
        }

        // POST: NganHangs/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult CreateNganHang([Bind(Include = "MaNganHang,TenNganHang,MaHinhThucTT")] NganHang nganHang)
        {
            if (ModelState.IsValid)
            {
                conn.NganHangs.Add(nganHang);
                conn.SaveChanges();
                return RedirectToAction("IndexNganHang");
            }

            ViewBag.MaHinhThucTT = new SelectList(conn.HinhThucThanhToans, "MaHinhThucTT", "TenHinhThucTT", nganHang.MaHinhThucTT);
            return View(nganHang);
        }


        //Delete Ngan Hang ----------------
        // POST: NganHang/Delete
        [HttpPost]

        public ActionResult DeleteNganHang(string id)
        {
            try
            {
                using (var conn = new DVMB())
                {
                    var obj = conn.NganHangs.Find(id);
                    conn.NganHangs.Remove(obj);
                    conn.SaveChanges();
                    return Json(new { message = "Success!" }, JsonRequestBehavior.AllowGet);
                }
            }
            catch (Exception e)
            {
                return Json(new { message = "Fail!" }, JsonRequestBehavior.AllowGet);
            }
        }
        //Khuyen mai ------------------------------------------

        public ActionResult IndexKhuyenMai()
        {
            return View(conn.KhuyenMais.ToList());
        }

        // GET: KhuyenMais/Create
        public ActionResult CreateKhuyenMai()
        {
            return View();
        }

        // POST: KhuyenMai/Create

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult CreateKhuyenMai([Bind(Include = "MaKhuyenMai,TenKhuyenMai,PhanTramSale,ThoiGianBD,ThoiGianKT")] KhuyenMai khuyenMai)
        {
            if (ModelState.IsValid)
            {
                conn.KhuyenMais.Add(khuyenMai);
                conn.SaveChanges();
                return RedirectToAction("IndexKhuyenMai");
            }

            return View(khuyenMai);
        }

        [HttpPost]

        public ActionResult DeleteKhuyenMai(string id)
        {
            try
            {
                using (var conn = new DVMB())
                {
                    var obj = conn.KhuyenMais.Find(id);
                    conn.KhuyenMais.Remove(obj);
                    conn.SaveChanges();
                    return Json(new { message = "Success!" }, JsonRequestBehavior.AllowGet);
                }
            }
            catch (Exception e)
            {
                return Json(new { message = "Fail!" }, JsonRequestBehavior.AllowGet);
            }
        }

    }    
 }



