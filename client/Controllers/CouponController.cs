using client.Interfaces;
using client.Models;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace client.Controllers
{
    public class CouponController : Controller
    {
        private readonly ICouponService _couponService;

        public CouponController(ICouponService couponService)
        {
            _couponService = couponService;
        }

        /// <summary>Retrieves a list of coupons from a service and returns a view with the list.</summary>
        /// <returns>An IActionResult representing the result of the operation.</returns>
        public async Task<IActionResult> CouponIndex()
        {
            List<CouponDto> list = new();

            ResponseDto response = await _couponService.GetAllCouponsAsync();

            if (response != null && response.Success)
            {
                list = JsonConvert.DeserializeObject<List<CouponDto>>(Convert.ToString(response.Result)!)!;
            }
            else
            {
                TempData["error"] = response!.Message;
            }

            return View(list);
        }

        /// <summary>Returns a view for creating a coupon.</summary>
        /// <returns>View result.</returns>
        public IActionResult CreateCoupon()
        {
            return View();
        }

        /// <summary>Handles the creation of a coupon by validating the model, calling
        /// the coupon service to create the coupon, and redirecting to the coupon
        /// index page if successful.</summary>
        /// <param name="couponDto">Represents the data needed to create a coupon.</param>
        /// <returns>An IActionResult representing the result of the operation.</returns>
        [HttpPost]
        public async Task<IActionResult> CreateCoupon(CouponDto couponDto)
        {
            if (ModelState.IsValid)
            {
                ResponseDto response = await _couponService.CreateCouponAsync(couponDto);

                if (response != null && response.Success)
                {
                    TempData["success"] = "Cupón creado correctamente";

                    return RedirectToAction(nameof(CouponIndex));
                }

                TempData["error"] = response!.Message;
            }

            return View(couponDto);
        }

        /// <summary>Retrieves a coupon by its ID and returns a view with the coupon
        /// details if it exists, otherwise it returns a not found error.</summary>
        /// <param name="id">The id of the coupon to deleted.</param>
        /// <returns>An IActionResult representing the result of the operation.</returns>
        public async Task<IActionResult> CouponDelete(int id)
        {
            ResponseDto response = await _couponService.GetCouponByIdAsync(id);

            if (response != null && response.Success)
            {
                CouponDto couponDto = JsonConvert.DeserializeObject<CouponDto>(Convert.ToString(response.Result)!)!;

                return View(couponDto);
            }

            TempData["error"] = response!.Message;

            return NotFound();
        }

        /// <summary>Handles the deletion of a coupon and redirects to the coupon index
        /// page if successful, or displays an error message if not.</summary>
        /// <param name="couponDto">The coupon to delete.</param>
        /// <returns>An IActionResult representing the result of the operation.</returns>
        [HttpPost]
        public async Task<IActionResult> CouponDelete(CouponDto couponDto)
        {
            ResponseDto response = await _couponService.DeleteCouponAsync(couponDto.CouponId);

            if (response != null && response.Success)
            {
                TempData["success"] = "Cupón eliminado correctamente";

                return RedirectToAction(nameof(CouponIndex));
            }

            TempData["error"] = response!.Message;

            return View(couponDto);
        }
    }
}