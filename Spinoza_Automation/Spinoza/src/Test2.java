import java.util.concurrent.TimeUnit;
import org.openqa.selenium.By;
import org.openqa.selenium.WebDriver;
import org.openqa.selenium.WebElement;
import org.openqa.selenium.chrome.ChromeDriver;
import org.openqa.selenium.support.ui.ExpectedConditions;
import org.openqa.selenium.support.ui.WebDriverWait;

public class Test2 {

	public static void main(String[] args) throws InterruptedException {
		// Invoke Browser
		// Test Edit Test
		
		System.setProperty("webdriver.chrome.driver", "E:\\Github\\Spinoza_Automation\\Spinoza\\lib\\chromedriver.exe");
		WebDriver driver = new ChromeDriver();
		driver.manage().timeouts().implicitlyWait(10, TimeUnit.SECONDS);

		driver.manage().window().maximize();
		driver.get("http://localhost:3000/");

		System.out.println("TEST START - EDIT");
		EditTest(driver);
		System.out.println("TEST END - EDIT");

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

			// EDIT DESCRIPTION
			WebElement Description = driver.findElement(By.cssSelector(".w-md-editor-text-input"));
			Description.click();
			Description.clear();
			Description.sendKeys("NEW EDIT DESCRIPTION");
			System.out.println("DESCRIPTION = 'NEW EDIT DESCRIPTION'");

			// EDIT TAGS
			int TagsCount = driver.findElements((By.xpath(
					"//button[@class='mantine-ActionIcon-transparent mantine-ActionIcon-root mantine-MultiSelect-defaultValueRemove mantine-9kufq0']")))
					.size();
			
			if(TagsCount>2) {
				driver.findElement(By.xpath(
						"(//button[@class='mantine-ActionIcon-transparent mantine-ActionIcon-root mantine-MultiSelect-defaultValueRemove mantine-9kufq0'])[1]"))
				.click();
				System.out.println("TAGS = ['C#','JavaScript','Python']");
			}

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

		} catch (Exception e) {
			System.out.println(e);
		}

	}
}
