using UnityEngine;

namespace SupanthaPaul
{
	public class InputSystem : MonoBehaviour
	{
		// input string caching
		static readonly string HorizontalInput = "Horizontal";
		static readonly string VerticalInput = "Vertical"; // Tambahan untuk naik turun tangga
		static readonly string JumpInput = "Jump";
		static readonly string DashInput = "Dash";
		
		// Tambahkan opsi untuk mengkonfigurasi tombol sprint melalui Inspector
		// Ini memungkinkan mengubah tombol tanpa mengubah kode
		[SerializeField] private static KeyCode sprintKey = KeyCode.LeftShift;
		
		// Singleton instance agar bisa dikonfigurasi melalui Inspector
		private static InputSystem _instance;
		
		private void Awake()
		{
			_instance = this;
		}

		public static float HorizontalRaw()
		{
			return Input.GetAxisRaw(HorizontalInput);
		}

		public static float VerticalRaw()
		{
			return Input.GetAxisRaw(VerticalInput);
		}

		public static bool Jump()
		{
			return Input.GetButtonDown(JumpInput);
		}

		public static bool Dash()
		{
			return Input.GetButtonDown(DashInput);
		}

		public static bool Sprint()
		{
			// Pendekatan 1: Menggunakan KeyCode langsung (direkomendasikan)
			return Input.GetKey(KeyCode.LeftShift);
			
			// Pendekatan 2: Jika ingin gunakan sprintKey yang dapat dikonfigurasi
			// return _instance != null ? Input.GetKey(sprintKey) : Input.GetKey(KeyCode.LeftShift);
			
			// Pendekatan 3: Menggunakan tombol Ctrl sebagai alternatif jika Shift bermasalah
			// return Input.GetKey(KeyCode.LeftControl);
		}
	}
}