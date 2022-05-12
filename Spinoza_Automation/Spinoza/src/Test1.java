import java.util.Date;
import java.util.concurrent.TimeUnit;
import org.openqa.selenium.By;
import org.openqa.selenium.Keys;
import org.openqa.selenium.WebDriver;
import org.openqa.selenium.WebElement;
import org.openqa.selenium.chrome.ChromeDriver;
import org.openqa.selenium.support.ui.ExpectedConditions;
import org.openqa.selenium.support.ui.WebDriverWait;

public class Test1 {

	public static void main(String[] args) throws InterruptedException {
		// Invoke Browser
		// Test Create Test

		System.setProperty("webdriver.chrome.driver", ".\\lib\\chromedriver.exe");
		WebDriver driver = new ChromeDriver();
		driver.manage().timeouts().implicitlyWait(10, TimeUnit.SECONDS);

		driver.manage().window().maximize();
		driver.get("http://localhost:3000/");

		System.out.println("TEST START - TEST CREATE");
		CreateNewTest(driver);
		System.out.println("TEST END - TEST CREATE");
		
		Thread.sleep(5000);
		driver.quit();
	}

	public static void CreateNewTest(WebDriver driver) throws InterruptedException {
		driver.findElement(By.xpath("//span[contains(text(),'Tests')]")).click();
		driver.findElement(By.xpath("//span[contains(text(),'NEW TEST')]")).click();

		// TITLE
		AddTitle(driver);

		// DESCRIPTION
		AddDescription(driver);

		// TAGS
		AddTags(driver);

		// SAVE
		SaveTest(driver);
	}

	private static void AddTitle(WebDriver driver) {
		try {
			Date date = new Date();
			String s = date.toLocaleString();
			WebElement TestTitle = driver.findElement(By.xpath("//input[@placeholder='Test title']"));
			TestTitle.clear();
			TestTitle.sendKeys("Automation TEST | " + s);
			System.out.println("TITLE = 'NEW TEST' | " + s);

		} catch (Exception e) {
			System.out.println("Error = Test Title");
			System.out.println(e);
		}
	}

	private static void AddDescription(WebDriver driver) {
		try {
			WebElement Description = driver.findElement(By.cssSelector(".w-md-editor-text-input"));
			Description.click();
			Description.clear();
			Description.sendKeys("TEST DESCRIPTION");
			System.out.println("DESCRIPTION = 'TEST DESCRIPTION'");

		} catch (Exception e) {
			System.out.println("Error = Description");
			System.out.println(e);
		}
	}

	private static void AddTags(WebDriver driver) {
		try {
			WebElement Tags = driver.findElement(By.xpath("//input[@placeholder='#Tags']"));
			Tags.sendKeys("React", Keys.ENTER);
			Tags.sendKeys("C#", Keys.ENTER);
			Tags.sendKeys("JavaScript", Keys.ENTER);
			Tags.sendKeys("Python", Keys.ENTER);
			System.out.println("TAGS = ['React','C#','JavaScript','Python']");

		} catch (Exception e) {
			System.out.println("Error = Tags");
			System.out.println(e);
		}
	}
	
	private static void SaveTest(WebDriver driver) {
		try {
			driver.findElement(By.cssSelector(
					"button[class='mantine-Button-gradient mantine-Button-root mantine-Group-child mantine-e8vnjr']"))
			.click();
			
			WebDriverWait wait = new WebDriverWait(driver, 15);
			wait.until(ExpectedConditions.visibilityOfElementLocated(By.xpath("//div[@role='alert']")));
			
			String alert = driver.findElement(By.xpath("//div[@role='alert']")).getText();
			
			if(alert.contains("InternalServerError")) {
				System.out.println("TEST FAILED");
			}
			else {
				System.out.println("TEST SUCCESS");
			}
			
			driver.findElement(By.xpath("//span[contains(text(),'Tests')]")).click();
			
		} catch (Exception e) {
			System.out.println("Error = Save As Draft");
			System.out.println(e);
		}
	}

}
