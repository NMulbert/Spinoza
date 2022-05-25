import java.util.concurrent.TimeUnit;

import org.openqa.selenium.By;
import org.openqa.selenium.Keys;
import org.openqa.selenium.WebDriver;
import org.openqa.selenium.WebElement;
import org.openqa.selenium.chrome.ChromeDriver;
import org.testng.Assert;

public class Test4 {

	public static void main(String[] args) throws InterruptedException {
		// Invoke Browser
		// Tests Search Test

		System.setProperty("webdriver.chrome.driver", ".\\lib\\chromedriver.exe");
		WebDriver driver = new ChromeDriver();
		driver.manage().timeouts().implicitlyWait(15, TimeUnit.SECONDS);

		driver.manage().window().maximize();
		driver.get("http://localhost:3000/");

		System.out.println("TEST START - TEST'S SEARCH");

		System.out.println("-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-");

		System.out.println("TEST START - TEST 1");
		SearchTestsByTag(driver, "Python");
		System.out.println("TEST END - TEST 1");

		System.out.println("-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-");

		System.out.println("TEST END - TEST'S SEARCH");

		Thread.sleep(3000);
		driver.quit();
	}

	public static void SearchTestsByTag(WebDriver driver, String tag) {
		try {
			// GET TESTS COUNT BY TAG
			int count = GetTestsByTag(driver, tag);

			// SEARCH TESTS BY TAG
			WebElement Search = driver.findElement(By.xpath("//input[@placeholder='Select tags']"));
			Search.sendKeys(tag, Keys.ENTER);

			Thread.sleep(2500);
			int result = driver.findElements(By.xpath("//div[@class='mantine-Col-root mantine-1akkebb']")).size() - 1;

			SearchTestsByTag_Check(count,result);
			
			driver.findElement(By.xpath("//button[@class='mantine-ActionIcon-transparent mantine-ActionIcon-root mantine-MultiSelect-defaultValueRemove mantine-cc6qwu']")).click();
			Search.click();
			
		} catch (Exception e) {
			System.out.println("Search Tests By Tag = FAILED");
			System.out.println(e);
		}
	}

	private static int GetTestsByTag(WebDriver driver, String tag) {
		try {
			// GET TESTS BY TAG
			return driver.findElements(By.xpath("//span[contains(text(),'" + tag + "')]")).size();

		} catch (Exception e) {
			System.out.println("Get Tests By Tag = FAILED");
			System.out.println(e);
			return -1;
		}
	}

	private static void SearchTestsByTag_Check(int count, int result) {
		try {
			Assert.assertEquals(count, result);
			System.out.println("Search Tests By Tag = SUCCESS");
		} catch (Exception e) {
			System.out.println("Search Tests By Tag = FAILED");
			System.out.println(e);
		}
	}
}
