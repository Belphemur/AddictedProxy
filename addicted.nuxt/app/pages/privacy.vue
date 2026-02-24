<template>
  <v-container fluid class="pa-2 pa-sm-4" style="max-width: 1600px">
    <v-sheet rounded="lg" color="rgba(0,0,0,0.75)" class="pa-4 pa-sm-6">
      <div class="text-center mb-4">
        <h1 class="text-h5 text-sm-h4 font-weight-bold">Privacy Policy</h1>
        <p class="text-body-2 text-sm-body-1 text-medium-emphasis mt-1">Updated: May 21, 2024</p>
      </div>

      <p class="text-body-2 text-sm-body-1">
        This Privacy Policy explains how gestdown.info ("we" or "us") collects, uses, and discloses information about
        visitors to our website. By using our website, you agree to the terms of this Privacy Policy.
      </p>

      <h2 class="text-h6 mt-4">1. Lawful Basis for Processing</h2>
      <p class="text-body-2 text-sm-body-1">
        Our processing of your personal data is based on your consent when you use our website.
      </p>

      <h2 class="text-h6 mt-4">2. Information We Collect</h2>
      <p class="text-body-2 text-sm-body-1">
        We do not collect any personal information from our visitors. We do not log IP addresses or collect any other
        information that could identify individuals. No account registration is required to use our services.
      </p>

      <h2 class="text-h6 mt-4">3. Use of Cookies</h2>
      <p class="text-body-2 text-sm-body-1">
        We do not use cookies or other tracking technologies on our website.
      </p>

      <h2 class="text-h6 mt-4">4. Third-Party Links</h2>
      <p class="text-body-2 text-sm-body-1">
        Our website may contain links to third-party websites not operated by us. We are not responsible for the
        privacy practices of these websites. We encourage you to review their privacy policies before using them.
      </p>

      <h2 class="text-h6 mt-4">5. Children's Privacy</h2>
      <p class="text-body-2 text-sm-body-1">
        Our website is not directed to children under 13. We do not knowingly collect personal information from
        children under 13.
      </p>

      <h2 class="text-h6 mt-4">6. Information Sharing</h2>
      <p class="text-body-2 text-sm-body-1">
        We do not share any visitor information with third-party websites or services, including Addic7ed.
      </p>

      <h2 class="text-h6 mt-4">7. CDN Usage</h2>
      <p class="text-body-2 text-sm-body-1">
        We use Cloudflare as a content delivery network (CDN) to improve the speed and security of our website.
        Cloudflare may collect and process certain information, including IP addresses and other data about website
        traffic, to provide and optimize their services. For more information, please visit Cloudflare's privacy
        policy.
      </p>

      <h2 class="text-h6 mt-4">8. Metrics Collection</h2>
      <p class="text-body-2 text-sm-body-1">
        We use Sentry.io to collect metrics and monitor the health of our system. Sentry.io may collect and process
        certain information, including error reports and performance data, to help us identify and fix issues. This
        data is not linked to any personal information and is used solely to maintain the quality and availability of
        our services. For more information, please visit Sentry.io's privacy policy.
      </p>

      <h2 class="text-h6 mt-4">9. Data Subject Rights</h2>
      <p class="text-body-2 text-sm-body-1">Under GDPR, you have the right to:</p>
      <div id="list">
        <ul class="text-body-2 text-sm-body-1">
          <li>Request access to your personal data.</li>
          <li>Request rectification or erasure of your personal data.</li>
          <li>Restrict or object to the processing of your personal data.</li>
          <li>Data portability.</li>
          <li>
            Withdraw consent at any time, without affecting the lawfulness of processing based on consent before its
            withdrawal.
          </li>
          <li>Lodge a complaint with a supervisory authority.</li>
        </ul>
      </div>
      <p class="text-body-2 text-sm-body-1">
        You can exercise these rights by contacting us at support@gestdown.info.
      </p>

      <h2 class="text-h6 mt-4">10. Data Retention</h2>
      <p class="text-body-2 text-sm-body-1">
        We do not retain any personal data as we do not collect any personal information from our visitors. Any
        requests for subtitles are processed in real-time and not stored on our servers.
      </p>

      <h2 class="text-h6 mt-4">11. International Data Transfers</h2>
      <p class="text-body-2 text-sm-body-1">
        Our website is operated from Canada. If you are accessing our website from outside Canada, please be aware that
        your information may be transferred to, stored, and processed in Canada. By using our website, you consent to
        this transfer, storage, and processing.
      </p>

      <h2 class="text-h6 mt-4">12. Changes to this Privacy Policy</h2>
      <p class="text-body-2 text-sm-body-1">
        We reserve the right to modify this Privacy Policy at any time. If we make material changes, we will notify you
        by posting the changes on our website.
      </p>

      <h2 class="text-h6 mt-4">13. Contact Us</h2>
      <p class="text-body-2 text-sm-body-1">
        If you have any questions about this Privacy Policy, please contact us at support@gestdown.info.
      </p>
    </v-sheet>
  </v-container>
</template>

<script lang="ts" setup>
import { mdiShieldAccount } from "@mdi/js";

definePageMeta({
  name: "Privacy Policy",
  order: 50,
  icon: mdiShieldAccount,
})
useSeoMeta({
  title: "Gestdown: Addic7ed Proxy",
  description: "Privacy Policy of Gestdown",
  ogDescription: "Privacy Policy of Gestdown",
  ogImage: "/img/logo.png"
})
</script>

<style scoped>
#list {
  padding-left: 20px;
}
</style>