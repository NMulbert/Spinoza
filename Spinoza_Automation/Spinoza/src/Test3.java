import java.util.concurrent.TimeUnit;
import org.openqa.selenium.By;
import org.openqa.selenium.WebDriver;
import org.openqa.selenium.chrome.ChromeDriver;
import org.openqa.selenium.support.ui.ExpectedConditions;
import org.openqa.selenium.support.ui.WebDriverWait;

public class Test3 {

	public static void main(String[] args) throws InterruptedException {
		// Invoke Browser
		// Test For Add Question To Test
		
		System.setProperty("webdriver.chrome.driver", "E:\\Github\\Spinoza_Automation\\Spinoza\\lib\\chromedriver.exe");
		WebDriver driver = new ChromeDriver();
		driver.manage().timeouts().implicitlyWait(10, TimeUnit.SECONDS);

		driver.manage().window().maximize();
		driver.get("http://localhost:3000/");

		System.out.println("TEST START - ADD TO QUESTION");
		EditTest(driver);
		System.out.println("TEST END - ADD TO QUESTION");

		Thread.sleep(3000);
		driver.quit();

	}

	public static void EditTest(WebDriver driver) throws InterruptedException {
		try {
			driver.findElement(By.xpath("//span[contains(text(),'Tests')]")).click();

			int TestCount = driver.findElements(By.xpath("//div[@class='mantine-Col-root mantine-1akkebb']")).size()
					- 1;
			System.out.println("TEST's COUNT = " + TestCount);

			driver.findElement(By.xpath("(//button[@class='mantine-Button-light mantine-Button-root mantine-5l7wha'])["
					+ (TestCount) + "]")).click();

			driver.findElement(By.xpath(".//*[@class='icon icon-tabler icon-tabler-edit']")).click();

			// CHOOSE FROM CATALOG
			driver.findElement(By.xpath(
					"//button[@class='mantine-Button-outline mantine-Button-root mantine-Group-child mantine-4sv719']"))
					.click();

			// SELECT QUESTION
			int QuestionCount = driver
					.findElements(By.xpath("//div[@class='mantine-Paper-root mantine-Card-root mantine-199wbki']"))
					.size();

			if (QuestionCount > 0) {
				for (int i = 1; i <= QuestionCount; i++) {
					driver.findElement(
							By.xpath("(//button[@class='mantine-Button-outline mantine-Button-root mantine-da2sop'])["
									+ (i) + "]"))
							.click();
				}
			}
			driver.findElement(By.cssSelector(
					"button[class='mantine-Button-filled mantine-Button-root mantine-Group-child mantine-9uo5n1']"))
					.click();

			// SAVE AS DRAFT
			driver.findElement(By.cssSelector(
					"button[class='mantine-Button-gradient mantine-Button-root mantine-Group-child mantine-1m2tbz']"))
					.click();

			WebDriverWait wait = new WebDriverWait(driver, 15);
			wait.until(ExpectedConditions.visibilityOfElementLocated(By.xpath("//div[@role='alert']")));

			String alert = driver.findElement(By.xpath("//div[@role='alert']")).getText();

			if (alert.contains("InternalServerError")) {
				System.out.println("TEST FAILED");
			} else {
				System.out.println("TEST SUCCESS");
			}

			driver.findElement(By.xpath("//span[contains(text(),'Tests')]")).click();

			// RE-ENTER TO TEST
			driver.findElement(By.xpath("(//button[@class='mantine-Button-light mantine-Button-root mantine-5l7wha'])["
					+ (TestCount) + "]")).click();

		} catch (Exception e) {
			System.out.println(e);
		}

	}

}
