import java.util.concurrent.TimeUnit;
import org.openqa.selenium.By;
import org.openqa.selenium.WebDriver;
import org.openqa.selenium.chrome.ChromeDriver;

public class Test4 {

	public static void main(String[] args) throws InterruptedException {
		// Invoke Browser
		// Tests Delete Test

		System.setProperty("webdriver.chrome.driver", ".\\lib\\chromedriver.exe");
		WebDriver driver = new ChromeDriver();
		driver.manage().timeouts().implicitlyWait(15, TimeUnit.SECONDS);

		driver.manage().window().maximize();
//		driver.get("http://localhost:3000/");
		driver.get("https://www.zionetapp.com/");

		System.out.println("-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-");
		System.out.println("TEST START - TEST'S DELETE");
		System.out.println("-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-");

		System.out.println("TEST START - TEST 1");
		DeleteAutomationTest(driver); // DELETE AUTOMATION TEST'S
		System.out.println("TEST END - TEST 1");

		System.out.println("-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-");
		Thread.sleep(5000);
		driver.quit();
	}

	// TEST'S DELETE TEST
	public static void DeleteAutomationTest(WebDriver driver) {
		try {
			// DELETE AUTOMATION TEST'S

			driver.findElement(By.xpath("//span[contains(text(),'Tests')]")).click();
			Thread.sleep(3000);
			int TestCount = driver.findElements(By.xpath("//div[@class='mantine-Col-root mantine-1akkebb']")).size()
					- 1;
			System.out.println("TEST's COUNT = " + TestCount);

			int count = driver.findElements(By.xpath("//h4[contains(text(),'Automation')]")).size();

			System.out.println("TEST's AUTOMATION COUNT = " + count);

			while (count != 0) {
				driver.findElement(By.xpath(
						"(//button[@class='mantine-Button-light mantine-Button-root mantine-Group-child mantine-1q8ef6b'])["
								+ (TestCount) + "]"))
						.click();

				driver.findElement(By.cssSelector(
						"div.mantine-Modal-root.mantine-bsiqi3 div.mantine-16pg774.mantine-Modal-inner div.mantine-Paper-root.mantine-Modal-modal.mantine-vos779:nth-child(1) div.mantine-l553vn.mantine-Modal-body div.mantine-Group-root.mantine-1u7f61y > button.mantine-Button-filled.mantine-Button-root.mantine-Group-child.mantine-6nsk2i:nth-child(1)"))
						.click();

				count--;
				TestCount--;

				Thread.sleep(3000);

				System.out.println("Delete Automation Tests = SUCCESS");
			}

		} catch (Exception e) {
			System.out.println("Error = DeleteAutomationTest");
			System.out.println(e);
		}
	}
}
