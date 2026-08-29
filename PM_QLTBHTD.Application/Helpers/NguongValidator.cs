using PM_QLTBHTD.Domain.Entities;

namespace PM_QLTBHTD.Application.Helpers
{
    public record NguongValidationIssue(string Loai, string MoTa);

    /// <summary>
    /// Config Validator — mục 3.1 "xac-nhan-chuan-hoa-kien-truc-cuoi.md": quét toàn bộ khoảng
    /// ngưỡng (CanDuoi..CanTren) của 1 chỉ tiêu, đảm bảo phủ kín trục số (-∞,+∞) và không có đoạn
    /// chồng lấn. Chỉ áp dụng cho ngưỡng dạng KHOẢNG GIÁ TRỊ (không BieuThuc_Logic) — ngưỡng đa
    /// biến (BieuThuc_Logic) không có khái niệm "khoảng" nên bỏ qua. Gộp theo MaKetQua vì 1 chỉ
    /// tiêu có thể có nhiều bảng ngưỡng độc lập (mỗi Formula/MaKetQua 1 bảng riêng).
    /// </summary>
    public static class NguongValidator
    {
        private static string Nhan(CBM_Nguong n) => $"Sᵢ={n.Diem_Si} (#{n.ID_Nguong})";

        public static List<NguongValidationIssue> KiemTraGapOverlap(IEnumerable<CBM_Nguong> tatCaNguong)
        {
            var issues = new List<NguongValidationIssue>();

            foreach (var nhomTheoMaKetQua in tatCaNguong.GroupBy(n => n.MaKetQua ?? ""))
            {
                var maKetQua = nhomTheoMaKetQua.Key;
                var tienTo = string.IsNullOrEmpty(maKetQua) ? "" : $"[Kết quả '{maKetQua}'] ";

                var khoang = nhomTheoMaKetQua
                    .Where(n => string.IsNullOrWhiteSpace(n.BieuThuc_Logic))
                    .OrderBy(n => n.CanDuoi ?? decimal.MinValue)
                    .ToList();

                if (khoang.Count == 0) continue; // toàn BieuThuc_Logic — không có khái niệm khoảng

                if (khoang[0].CanDuoi is not null)
                    issues.Add(new NguongValidationIssue("GAP",
                        $"{tienTo}Thiếu ngưỡng cho giá trị < {khoang[0].CanDuoi} (chưa phủ tới -∞)."));

                for (int i = 0; i < khoang.Count - 1; i++)
                {
                    var prev = khoang[i];
                    var curr = khoang[i + 1];

                    if (prev.CanTren is null)
                    {
                        issues.Add(new NguongValidationIssue("OVERLAP",
                            $"{tienTo}Ngưỡng '{Nhan(prev)}' không có cận trên (phủ tới +∞) " +
                            $"nhưng vẫn còn ngưỡng '{Nhan(curr)}' phía sau — 2 ngưỡng chồng lấn."));
                        continue;
                    }

                    if (prev.CanTren < curr.CanDuoi)
                    {
                        issues.Add(new NguongValidationIssue("GAP",
                            $"{tienTo}Khoảng trống giữa {prev.CanTren} và {curr.CanDuoi} — giá trị đo rơi vào đây sẽ không khớp ngưỡng nào."));
                    }
                    else if (prev.CanTren > curr.CanDuoi)
                    {
                        issues.Add(new NguongValidationIssue("OVERLAP",
                            $"{tienTo}Ngưỡng '{Nhan(prev)}' và '{Nhan(curr)}' " +
                            $"chồng lấn trong khoảng ({curr.CanDuoi}, {prev.CanTren})."));
                    }
                    else // prev.CanTren == curr.CanDuoi — điểm biên phải khớp ĐÚNG 1 trong 2 ngưỡng bao gồm
                    {
                        var prevBaoGom = prev.CanTren_BaoGom;
                        var currBaoGom = curr.CanDuoi_BaoGom;
                        if (prevBaoGom && currBaoGom)
                            issues.Add(new NguongValidationIssue("OVERLAP",
                                $"{tienTo}Giá trị đúng bằng {prev.CanTren} khớp CẢ 2 ngưỡng '{Nhan(prev)}' và " +
                                $"'{Nhan(curr)}' (cả 2 đều đánh dấu bao gồm biên)."));
                        else if (!prevBaoGom && !currBaoGom)
                            issues.Add(new NguongValidationIssue("GAP",
                                $"{tienTo}Giá trị đúng bằng {prev.CanTren} không khớp ngưỡng nào (cả 2 ngưỡng đều không bao gồm biên này)."));
                    }
                }

                if (khoang[^1].CanTren is not null)
                    issues.Add(new NguongValidationIssue("GAP",
                        $"{tienTo}Thiếu ngưỡng cho giá trị > {khoang[^1].CanTren} (chưa phủ tới +∞)."));
            }

            return issues;
        }
    }
}
