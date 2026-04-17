<template>
  <v-container fluid :class="layout.classes.pageContainer" :style="{ maxWidth: layout.maxWidth }">
    <v-sheet rounded="lg" :color="layout.colors.primaryPanel" :class="layout.classes.primaryPanel">
      <div :class="layout.classes.pageHeading">
        <h1 :class="layout.classes.pageTitle">DMCA Policy</h1>
        <p :class="layout.classes.pageSubtitle">Digital Millennium Copyright Act Notice</p>
      </div>

      <p :class="layout.classes.bodyText">
        Gestdown (gestdown.info) is a subtitle search and download service. We respect intellectual property rights
        and respond promptly to valid DMCA takedown notices.
      </p>

      <h2 :class="layout.classes.sectionHeading">1. About Our Service</h2>
      <p :class="layout.classes.bodyText">
        Gestdown lets you search and directly download legal subtitle files translated by users for TV shows and
        movies, served from our secure servers. We store subtitle files only — we do not offer, link to, or host
        any video, audio, torrent, or other copyrighted media files. We do not provide any means to source TV
        shows or movies. We carefully review content to ensure copyright compliance and will promptly remove any
        questionable files brought to our attention.
      </p>
      <p :class="layout.classes.bodyText">
        <strong>Important:</strong> We can only remove subtitle files from our website. We have no control over
        content on other websites, even if those sites reference or mention Gestdown.
      </p>
      <p :class="layout.classes.bodyText">
        Gestdown content is for personal use only. Commercial use is strictly prohibited.
      </p>

      <h2 :class="layout.classes.sectionHeading">2. Content Attribution</h2>
      <p :class="layout.classes.bodyText">
        Artwork, images, and media information (such as show posters and descriptions) displayed on this website
        are sourced from
        <a href="https://www.themoviedb.org" target="_blank" rel="noopener noreferrer">The Movie Database (TMDB)</a>.
        This content is contributed by the community at TMDB and is used in accordance with
        <a href="https://www.themoviedb.org/api-terms-of-use" target="_blank" rel="noopener noreferrer">TMDB's API terms of use</a>.
        Gestdown is not endorsed by or affiliated with TMDB.
      </p>

      <h2 :class="layout.classes.sectionHeading">3. Copyright Infringement</h2>
      <p :class="layout.classes.bodyText">
        If you believe that your copyrighted work has been reproduced on this site in a way that constitutes
        copyright infringement, please submit a written DMCA takedown notice to our designated agent using the
        contact information below.
      </p>

      <h2 :class="layout.classes.sectionHeading">4. DMCA Takedown Procedure</h2>
      <p :class="layout.classes.bodyText">
        To file a valid DMCA notice, please include all of the following:
      </p>
      <div class="dmca-list">
        <ul :class="layout.classes.bodyText">
          <li>Your physical or electronic signature as the copyright owner or their authorized representative.</li>
          <li>A description of the copyrighted work(s) you claim has been infringed.</li>
          <li>
            Identification of the allegedly infringing material and information sufficient to allow us to locate
            it on the site (e.g., a URL).
          </li>
          <li>Your contact information: name, mailing address, telephone number, and email address.</li>
          <li>
            A statement that you have a good faith belief that use of the material is not authorized by the
            copyright owner, its agent, or the law.
          </li>
          <li>
            A statement, made under penalty of perjury, that the information in your notice is accurate and that
            you are the copyright owner or are authorized to act on their behalf.
          </li>
        </ul>
      </div>

      <h2 :class="layout.classes.sectionHeading">5. Counter-Notice</h2>
      <p :class="layout.classes.bodyText">
        If you believe your content was removed in error or due to misidentification, you may submit a
        counter-notice. A valid counter-notice must include:
      </p>
      <div class="dmca-list">
        <ul :class="layout.classes.bodyText">
          <li>Your physical or electronic signature.</li>
          <li>Identification of the removed material and its former location on the site.</li>
          <li>
            A statement under penalty of perjury that you have a good faith belief the material was removed due
            to a mistake or misidentification.
          </li>
          <li>
            Your name, address, and telephone number, and a statement that you consent to the jurisdiction of
            the federal district court in your area.
          </li>
        </ul>
      </div>

      <h2 :class="layout.classes.sectionHeading">6. Repeat Infringers</h2>
      <p :class="layout.classes.bodyText">
        In accordance with the DMCA and other applicable law, Gestdown has adopted a policy of terminating,
        in appropriate circumstances, access for users who are deemed to be repeat infringers.
      </p>

      <h2 :class="layout.classes.sectionHeading">7. Contact Our DMCA Agent</h2>
      <p :class="layout.classes.bodyText">
        To submit a takedown request or a counter-notice, please write to our DMCA agent at the following
        address. The email is displayed as an image to reduce automated abuse:
      </p>
      <div class="email-wrapper mt-3 mb-3">
        <canvas ref="emailCanvas" :width="canvasWidth" height="34"></canvas>
        <noscript>
          <p :class="layout.classes.bodyText">Please enable JavaScript to view the contact email address.</p>
        </noscript>
      </div>
      <p :class="layout.classes.bodyText">
        We aim to respond to all valid requests within <strong>5–10 business days</strong>.
      </p>
    </v-sheet>
  </v-container>
</template>

<script lang="ts" setup>
import { mdiGavel } from "@mdi/js";
import { usePageLayout } from "~/composables/usePageLayout";

const layout = usePageLayout();

// useState inside the composable ensures the same word is used on both server and
// client (prevents hydration mismatch). With swr: 3600 in routeRules, the server
// renders this once and the cached page (including the same word) is served to all
// visitors for up to one hour.
const { canvasRef: emailCanvas, canvasWidth } = useObfuscatedEmail("dmca-email-word", "dmca", "admincmd.com");

definePageMeta({
  name: "DMCA",
  order: 60,
  icon: mdiGavel,
});

useSeoMeta({
  title: "Gestdown: DMCA Policy",
  description: "DMCA Policy and copyright takedown procedure for Gestdown",
  ogDescription: "DMCA Policy and copyright takedown procedure for Gestdown",
  ogImage: "/img/logo.png",
});
</script>

<style scoped>
.dmca-list {
  padding-left: 20px;
}

.email-wrapper {
  display: inline-block;
}

canvas {
  display: block;
}

a {
  color: rgb(var(--v-theme-secondary));
}
</style>
