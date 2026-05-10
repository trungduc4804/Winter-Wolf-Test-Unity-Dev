# Math-3 — Tile Matching Game

## Tổng quan dự án

Dự án này ban đầu được xây dựng theo cơ chế **Candy Crush** (hoán đổi vị trí 2 viên kẹo để tạo thành bộ 3). Sau quá trình phát triển, game đã được chuyển đổi hoàn toàn sang cơ chế **Sheep-a-Sheep / Tile Master** — người chơi **bấm vào item để đưa xuống khay**, khi khay có 3 item cùng loại thì tự động xóa.

---

## Cơ chế chơi hiện tại

- Bàn cờ 2D chứa các item (cá) thuộc nhiều loại khác nhau.
- Người chơi **click vào item** trên bàn cờ để đưa xuống **khay chứa 5 ô** ở phía dưới.
- Khi khay có **3 item cùng loại**, chúng tự động biến mất (match).
- Nếu khay đầy 5 ô mà không có bộ 3 khớp → **Game Over** (thua).
- Dọn sạch toàn bộ bàn cờ → **Game Win** (thắng).

---

## Các chế độ chơi

| Chế độ | Nút bấm | Mô tả |
|---|---|---|
| **Moves** | `btnMoves` | Giới hạn số lượt click. Hết lượt → thua. |
| **Time Challenge** | `btnTimeChallenge` | Đếm ngược 60 giây. Hết giờ → thua. Cho phép trả item về bàn cờ. |
| **Auto Win** | `btnAutoWin` | Game tự chơi theo chiến thuật thắng, delay 0.5s/lượt. |
| **Auto Lose** | `btnAutoLose` | Game tự chơi theo chiến thuật thua, delay 0.5s/lượt. |

---

## Lịch sử thay đổi

### Giai đoạn 1 — Chuyển đổi cơ chế cốt lõi (Candy Crush → Tile Master)

#### Thêm `SlotTray.cs` (File mới)
- Tạo hệ thống **khay chứa 5 ô** ở phía dưới màn hình.
- Khay sử dụng cùng prefab `PREFAB_CELL_BACKGROUND` với bàn cờ.
- Khi item được thêm vào, khay **tự động sắp xếp** theo loại để nhóm các item cùng loại lại.
- Khi có **3 item cùng loại** liền kề trong khay → tự động xóa (ExplodeView + RemoveMatch).
- Cung cấp `GetItemAtPosition(Vector2)` để phát hiện click vào item trong khay.

#### Sửa `BoardController.cs`
- **Bỏ cơ chế swap** (hoán đổi 2 ô) của Candy Crush.
- **Thêm cơ chế click** — mỗi click vào item trên bàn cờ đưa item đó xuống khay.
- Lưu `item.OriginalCell` khi lấy item ra khỏi bàn cờ (phục vụ tính năng hoàn tác).
- Tích hợp kiểm tra `m_slotTray.IsFull` để quyết định khi nào báo Game Over.

#### Sửa `Item.cs`
- Thêm thuộc tính `OriginalCell` để lưu vị trí xuất phát của item khi bị lấy xuống khay.
- Thêm `DOKill()` trước `DOMove` trong `AnimationMoveToPosition()` để tránh xung đột animation.

---

### Giai đoạn 2 — Thêm chế độ Time Challenge

#### Thêm `LevelTime.cs` (mở rộng)
- Đếm ngược thời gian thực (`Time.deltaTime`).
- Hiển thị `TIME: XX` lên Text UI.
- Khi hết giờ → gọi `OnConditionComplete()` → Game Over.

#### Sửa `GameManager.cs`
- Thêm thuộc tính `IsTimeChallengeMode` (bool) để các script khác biết chế độ hiện tại.
- Thêm hàm `LoadLevelTimeChallenge()` — khởi tạo `LevelTime` với 60 giây.
- **Sửa lỗi nghiêm trọng**: Đổi `Destroy(m_levelCondition.gameObject)` thành `Destroy(m_levelCondition)` để tránh xóa nhầm GameManager (vì LevelCondition là Component trên cùng GameObject).

#### Sửa `BoardController.cs` — Logic Time Challenge
- Trong Time Challenge mode, khi khay đầy → **không báo Game Over** (khác chế độ thường).
- Thêm tính năng **hoàn tác**: Click vào item trong khay → trả item đó về đúng ô xuất phát trên bàn cờ.
- Phát hiện click trong **vùng khay** bằng cách kiểm tra `clickPos.y <= m_slotTray.TrayAreaMinY + 1.6f` để tránh nhầm lẫn khi click lên bàn cờ.

---

### Giai đoạn 3 — Cải thiện hệ thống UI

#### Sửa `UIPanelMain.cs`
- **Bỏ cơ chế tự động tạo Button bằng code**.
- Chuyển sang dùng `[SerializeField]` — cho phép gán Button trực tiếp trong Unity Inspector.
- Hỗ trợ 5 nút: `btnMoves`, `btnTimer`, `btnAutoWin`, `btnAutoLose`, `btnTimeChallenge`.

#### Sửa `UIPanelGame.cs`
- Thêm `[SerializeField] private GameObject levelConditionPanel` — tham chiếu đến khung nền của bộ đếm thời gian.
- Thêm method `ShowConditionPanel(bool)` để ẩn/hiện toàn bộ panel (không chỉ text bên trong).
- Mặc định **ẩn panel** khi vào game (`Show()` gọi `ShowConditionPanel(false)`).
- Chỉ hiện panel khi vào **Time Challenge mode**.

#### Sửa `LevelCondition.cs`
- Thêm null-check cho `m_txt` trong tất cả các overload của `Setup()`.
- `OnDestroy()` tự động ẩn text và panel khi kết thúc ván.

#### Sửa `LevelMoves.cs` và `LevelTime.cs`
- Thêm `if (m_txt == null) return;` trong `UpdateText()` để tránh crash khi không có UI text.

#### Sửa `UIMainManager.cs`
- Thêm hàm `GetGamePanel()` để lấy tham chiếu đến `UIPanelGame`.
- `LoadLevelTimeChallenge()` gọi `ShowConditionPanel(true)` sau khi vào chế độ.
- Chế độ thường (Moves, Timer, Auto) **không hiển thị** panel bộ đếm.
- Thêm `LoadLevelTimeChallenge()` propagate event từ UI đến GameManager.

---

### Giai đoạn 4 — Sửa lỗi Animation và Click Detection

#### Sửa `SlotTray.cs`
- **Bỏ hiệu ứng rung/nảy** `DOJump` → đổi sang `DOMove` (trượt mượt mà).
- Thêm `DOKill()` trước mỗi `DOMove` để hủy animation cũ trước khi chạy animation mới, tránh tình trạng item bị dồn cục về cùng 1 ô.
- Thêm `TrayAreaMinY` property — xác định vùng Y tối thiểu của khay để phát hiện click chính xác.

#### Sửa click detection trong `BoardController.cs`
- **Vấn đề cũ**: Điều kiện `else if (hit.collider == null)` khiến việc click vào item trong khay rất khó (do collider của tray slot background bị hit trước).
- **Vấn đề mới sau fix**: Điều kiện `!clickedCell` kích hoạt logic hoàn tác ngay cả khi click vào ô trống trên bàn cờ → item tự động trở về bàn.
- **Giải pháp cuối cùng**: Kiểm tra `clickPos.y <= m_slotTray.TrayAreaMinY + 1.6f` — chỉ xử lý hoàn tác khi click **thực sự nằm trong vùng khay**, bất kể collider nào bị hit.

---

## Cấu trúc file quan trọng

```
Assets/
  Scripts/
    Board/
      Board.cs          — Logic tạo và quản lý bàn cờ
      Cell.cs           — Đại diện 1 ô trên bàn cờ
      Item.cs           — Base class cho tất cả item (có OriginalCell)
      NormalItem.cs     — Item thông thường (cá các màu)
      BonusItem.cs      — Item đặc biệt (bonus)
    Controllers/
      GameManager.cs        — Quản lý trạng thái game tổng thể
      BoardController.cs    — Xử lý click, điều phối bàn cờ và khay
      SlotTray.cs           — Hệ thống khay 5 ô phía dưới
      LevelCondition.cs     — Base class cho điều kiện thắng/thua
      LevelTime.cs          — Chế độ đếm ngược thời gian
      LevelMoves.cs         — Chế độ giới hạn số lượt
    UI/
      UIMainManager.cs      — Điều phối các panel UI
      UIPanelMain.cs        — Panel menu chính (chọn chế độ)
      UIPanelGame.cs        — Panel in-game (pause + timer)
      UIPanelPause.cs       — Panel tạm dừng
      UIPanelGameOver.cs    — Panel thua
      UIPanelWin.cs         — Panel thắng
```

---

## Hướng dẫn Setup Unity Inspector

### Canvas → PanelHome (`UIPanelMain`)
| Field | Gán vào |
|---|---|
| Btn Moves | `PanelHome/btnMoves` |
| Btn Auto Win | `SelectMode/Mode/btnAutoWin` |
| Btn Auto Lose | `SelectMode/Mode/btnAutoLose` |
| Btn Time Challenge | `SelectMode/Mode/btnTimeChallenge` |

### Canvas → PanelGame (`UIPanelGame`)
| Field | Gán vào |
|---|---|
| Level Condition View | `PanelGame/Time/Text` |
| Level Condition Panel | `PanelGame/Time` |
| Btn Pause | `PanelGame/btnPause` |

---

## Công nghệ sử dụng

- **Unity** (2D)
- **C#**
- **DOTween** — thư viện animation (DOMove, DOScale, DOKill)
- **Physics2D.Raycast** — phát hiện click trên bàn cờ
- **Unity UI** (uGUI) — hệ thống Button, Text

---
