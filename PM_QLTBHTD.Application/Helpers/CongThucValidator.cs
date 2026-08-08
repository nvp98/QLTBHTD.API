namespace PM_QLTBHTD.Application.Helpers
{
    public record VongLapKetQua(bool CoVongLap, List<int> DuongDi);

    /// <summary>
    /// Config Validator — mục 3.2 "xac-nhan-chuan-hoa-kien-truc-cuoi.md": dò tham chiếu vòng trong
    /// cây Aggregation (CBM_CongThuc_Bien.NguonBien='NHOM_CON'). ScoringEngine đã tự chặn vòng lặp
    /// LÚC CHẠY (duongDi + MaxDepth, ném VongLapNhomChiTieuException) — validator này chặn SỚM HƠN,
    /// ngay lúc lưu cấu hình, để người dùng biết ngay thay vì chỉ phát hiện khi có phiếu thật chạy vào.
    /// </summary>
    public static class CongThucValidator
    {
        /// <summary>Quét TOÀN BỘ đồ thị (nhóm → nhóm con) tìm vòng lặp bất kỳ — dùng cho endpoint
        /// chẩn đoán cây, quét định kỳ hoặc theo yêu cầu.</summary>
        public static VongLapKetQua TimVongLap(Dictionary<int, List<int>> canhTuNhomDenNhomCon)
        {
            var trangThai = new Dictionary<int, int>(); // 0=chưa thăm, 1=đang thăm (gray), 2=xong (black)

            foreach (var start in canhTuNhomDenNhomCon.Keys)
            {
                if (trangThai.GetValueOrDefault(start, 0) != 0) continue;

                var duongDi = new List<int>();
                var ketQua = Dfs(start, canhTuNhomDenNhomCon, trangThai, duongDi);
                if (ketQua != null) return new VongLapKetQua(true, ketQua);
            }
            return new VongLapKetQua(false, new List<int>());
        }

        private static List<int>? Dfs(int node, Dictionary<int, List<int>> edges, Dictionary<int, int> state, List<int> path)
        {
            state[node] = 1;
            path.Add(node);

            foreach (var next in edges.GetValueOrDefault(node) ?? new List<int>())
            {
                var s = state.GetValueOrDefault(next, 0);
                if (s == 1)
                {
                    var idx = path.IndexOf(next);
                    var chuTrinh = path.Skip(idx).ToList();
                    chuTrinh.Add(next);
                    return chuTrinh;
                }
                if (s == 0)
                {
                    var found = Dfs(next, edges, state, path);
                    if (found != null) return found;
                }
            }

            path.RemoveAt(path.Count - 1);
            state[node] = 2;
            return null;
        }

        /// <summary>Kiểm tra sớm TRƯỚC KHI lưu 1 cạnh mới (tuNhom → denNhom, vd sắp thêm biến
        /// NHOM_CON trỏ tới denNhom trong công thức của tuNhom): nếu denNhom đã có đường đi ngược
        /// về tới tuNhom trong đồ thị hiện có, thêm cạnh mới sẽ tạo vòng lặp.</summary>
        public static bool SeTaoVongLapNeuThemCanh(Dictionary<int, List<int>> canhHienCo, int tuNhom, int denNhom)
        {
            if (tuNhom == denNhom) return true; // tự tham chiếu chính nó

            var daTham = new HashSet<int>();
            var hangDoi = new Queue<int>();
            hangDoi.Enqueue(denNhom);
            daTham.Add(denNhom);

            while (hangDoi.Count > 0)
            {
                var node = hangDoi.Dequeue();
                if (node == tuNhom) return true;

                foreach (var next in canhHienCo.GetValueOrDefault(node) ?? new List<int>())
                {
                    if (daTham.Add(next)) hangDoi.Enqueue(next);
                }
            }
            return false;
        }
    }
}
